using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace WarrantyItemScan
{
    public partial class Form1 : Form
    {
        List<DO> boxDO = new List<DO>();
        List<Item> boxItem = new List<Item>();

        bool clearText = false;
        const string SELECT_LABEL = "--Select";
        const string CONFIRM_SAVE = "Confirm To Save"; 
        const string CONFIRM_DELETE = "Confirm To Delete";
        //const string SQL_CONNECTION = "Data Source=192.168.8.100;Initial Catalog=Hibex;User ID=sa;Password=@sysmanager;Persist Security Info=True";
        const string SQL_CONNECTION = "Data Source=192.168.1.187;Initial Catalog=HibexClone;User ID=sa;Password=@sysmanager";
        //const string SQL_CONNECTION = "Data Source=192.168.1.249;Initial Catalog=Hibex;User ID=sa;Password=@sysmanager;Persist Security Info=True";
        //const string SQL_CONNECTION = "Data Source=192.168.0.219;Initial Catalog=Hibex;User ID=fluke;Password=callmyname;Persist Security Info=True";
        //const string SQL_CONNECTION = "Data Source=localhost;Initial Catalog=Hibex;Persist Security Info=True";
        List<Warranty> list = new List<Warranty>();
        List<string> box = new List<string>();
        string tmpWarrantyID = string.Empty;
        public Form1()
        {
            InitializeComponent();
        }
        private string genrerateLabelIndexing(string label)
        {
            return SELECT_LABEL + " " + label + "--";
        }
        private List<Warranty> RefreshGrid(string selDO, string item)
        {
            dgv_mWarranty.DataSource = null;

            string qry = @"
DROP TABLE IF EXISTS #tmp
;WITH RecursiveCTE AS (
    SELECT m.do_no
    , m.item
    , m.status
    , m.req_qty
    ,1 Iteration
    FROM picking_matls m
    INNER JOIN master_items i
    ON i.item = m.item COLLATE SQL_Latin1_General_CP1_CS_AS
    WHERE ISNULL(i.warranty_period, 0) <> 0
    AND do_no = @seldo COLLATE SQL_Latin1_General_CP1_CS_AS
    AND (status = 1 AND pick_qty = req_qty)
    UNION ALL  
    SELECT m.do_no
    , m.item
    , m.status
    , m.req_qty
    , m.Iteration + 1
    FROM RecursiveCTE m
    WHERE m.Iteration < m.req_qty
)
SELECT ROW_NUMBER() OVER (PARTITION BY item ORDER BY item) line_number
, item
INTO #tmp
FROM RecursiveCTE

SELECT t.line_number, wt.wanranty_id, wt.id
FROM #tmp t
LEFT JOIN (
SELECT 
     CAST(ROW_NUMBER() OVER (PARTITION BY item ORDER BY id) AS INT) line_number
    , item
    , CAST(id AS INT) id
    , wanranty_id
    , issue_date
    FROM wanranty_monitors
    WHERE do_no = @seldo COLLATE SQL_Latin1_General_CP1_CS_AS
    AND item = @item COLLATE SQL_Latin1_General_CP1_CS_AS
) wt
ON t.item = wt.item COLLATE SQL_Latin1_General_CP1_CS_AS
AND t.line_number = wt.line_number
WHERE t.item = @item";
            List<SqlParameter> param = new List<SqlParameter>();
            param.Add(new SqlParameter("@seldo", SqlDbType.NVarChar) { Value = selDO });
            param.Add(new SqlParameter("@item", SqlDbType.NVarChar) { Value = item });
            List<Warranty> ls = ExecuteSelectQuery(qry, param).AsEnumerable()
                .Select(r => new Warranty(r.Field<long>(0), r.Field<string>(1), r.Field<int?>(2))).ToList();
            return ls;
        }

        private void AdjustStatusPickingJob(string dono, bool onSaved)
        {
            string qry = string.Empty;
            if (onSaved)
            {
                qry = @"
UPDATE pj
SET pj.modified_date = wa.latest_scanned_item
,   pj.status = 1
FROM picking_jobs pj
INNER JOIN (
    SELECT 
        wm.do_no
        ,COUNT(wm.do_no) AS all_scanned_items
        ,(  SELECT CAST(SUM(req_qty) AS INT) 
            FROM picking_matls pm 
            INNER JOIN master_items mi
            ON pm.item = mi.item COLLATE SQL_Latin1_General_CP1_CS_AS
            AND ISNULL(mi.warranty_period, 0) <> 0
            WHERE do_no = wm.do_no) AS all_picked_items
        , MAX(wm.issue_date) latest_scanned_item
    FROM 
        wanranty_monitors wm
    WHERE 
        wm.do_no = @dono COLLATE SQL_Latin1_General_CP1_CS_AS
    GROUP BY
        wm.do_no
) wa
ON pj.do_no = wa.do_no COLLATE SQL_Latin1_General_CP1_CS_AS
WHERE wa.all_picked_items = wa.all_scanned_items";
            }
            else
            {
                qry = @"
UPDATE picking_jobs
SET status = 0
,   modified_date = NULL
WHERE do_no = @dono
AND status = 1
AND modified_date IS NOT NULL";
            }
            List<SqlParameter> param = new List<SqlParameter>();
            param.Add(new SqlParameter("@dono", SqlDbType.NVarChar) { Value = dono });
            try
            {
                int aff = ExecuteNonQuery(qry, param);
                int idx = cbx_item.SelectedIndex;
                if (aff > 0)
                {
                    if (!onSaved)
                    {

                    }
                    //if (isUpdate)
                    //{
                    //    var a = boxDO[cbx_selDO.SelectedIndex];
                    //    cbx_selDO.Items[idx] = boxDO[idx];

                    //}
                    //btn_refresh_Click(null, null);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("got error: " + ex.Message, "Adjust Picking Job Status", MessageBoxButtons.OK, MessageBoxIcon.Hand, MessageBoxDefaultButton.Button1);
            }
        }

        private void btn_confirm_Click(object sender, EventArgs e)
        {
            string state = btn_confirm.Text;
            string warrantyId = tbx_txtScan.Text.Trim(); //string.IsNullOrEmpty(tmpWarrantyID) ? tbx_txtScan.Text.Trim() : tmpWarrantyID;
            tbx_txtScan.TextChanged -= tbx_txtScan_TextChanged;

            // check serial warranty length
            if (warrantyId.Length > 50)
            {
                MessageBox.Show("WarrantyID length is out of maximum length.",
                "Manage Data Process",
                MessageBoxButtons.OK,
                MessageBoxIcon.Hand,
                MessageBoxDefaultButton.Button1);
                return;
            }

            if (MessageBox.Show("Are you sure to " + (state.Equals(CONFIRM_SAVE) ? "save" : "delete") + " warranty?",
                "Manage Data Process",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question,
                MessageBoxDefaultButton.Button1) == DialogResult.Yes)
            {
                //string doNo = cbx_selDO.SelectedItem.ToString();
                //string item = box[cbx_item.SelectedIndex];
                string doNo = boxDO[cbx_selDO.SelectedIndex].DOcode;
                string item = boxItem[cbx_item.SelectedIndex].item;
                long lineNum = Convert.ToInt64(dgv_mWarranty[dgv_mWarranty.CurrentRowIndex, 0]) - 1;
                var recNoc = dgv_mWarranty[dgv_mWarranty.CurrentRowIndex, 2];
                string recNo = (recNoc == null ? string.Empty : recNoc.ToString());
                int idx = cbx_item.SelectedIndex;
                //* state control
                string qry = string.Empty;
                bool isInsert = false;
                List<SqlParameter> param = new List<SqlParameter>();
                if (state.Equals(CONFIRM_SAVE))
                {
                    if (string.IsNullOrEmpty(recNo))
                    {
                        // checked exists
                        qry = "SELECT TOP 1 wanranty_id FROM wanranty_monitors WHERE do_no = @dono COLLATE SQL_Latin1_General_CP1_CS_AS AND item = @item AND wanranty_id = @warranty COLLATE SQL_Latin1_General_CP1_CS_AS";
                        param.Add(new SqlParameter("@dono", SqlDbType.NVarChar) { Value = doNo });
                        param.Add(new SqlParameter("@item", SqlDbType.NVarChar) { Value = item });
                        param.Add(new SqlParameter("@warranty", SqlDbType.NVarChar) { Value = warrantyId });
                        try
                        {
                            var existsLs = ExecuteSelectQuery(qry, param).AsEnumerable().ToList();
                            if (existsLs.Count > 0)
                            {
                                MessageBox.Show("WarrantyID has exists in database.",
                                "Manage Data Process",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Hand,
                                MessageBoxDefaultButton.Button1);
                                tbx_txtScan.Focus();
                                return;
                            }
                            else
                            {
                                param.Clear();
                                qry = @"INSERT INTO wanranty_monitors(do_no, item, wanranty_id, issue_date) VALUES(@dono, @item, @warranty, @datenow)";
                                param.Add(new SqlParameter("@dono", SqlDbType.NVarChar) { Value = doNo });
                                param.Add(new SqlParameter("@item", SqlDbType.NVarChar) { Value = item });
                                param.Add(new SqlParameter("@warranty", SqlDbType.NVarChar) { Value = warrantyId });
                                param.Add(new SqlParameter("@datenow", SqlDbType.DateTime) { Value = DateTime.Now.ToLocalTime() });
                                isInsert = true;
                            }
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("fail get data: " + ex.Message,
                                "Get Data Failure", MessageBoxButtons.OK,
                                MessageBoxIcon.Hand,
                                MessageBoxDefaultButton.Button1);
                        }
                    }
                    else
                    {
                        // checked exists
                        qry = "SELECT TOP 1 wanranty_id FROM wanranty_monitors WHERE do_no = @dono COLLATE SQL_Latin1_General_CP1_CS_AS AND item = @item AND wanranty_id = @warranty COLLATE SQL_Latin1_General_CP1_CS_AS";
                        param.Add(new SqlParameter("@dono", SqlDbType.NVarChar) { Value = doNo });
                        param.Add(new SqlParameter("@item", SqlDbType.NVarChar) { Value = item });
                        param.Add(new SqlParameter("@warranty", SqlDbType.NVarChar) { Value = warrantyId });
                        try
                        {
                            var existsLs = ExecuteSelectQuery(qry, param).AsEnumerable().ToList();
                            if (existsLs.Count > 0)
                            {
                                MessageBox.Show("WarrantyID has exists in database.",
                                "Manage Data Process",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Hand,
                                MessageBoxDefaultButton.Button1);
                                tbx_txtScan.Focus();
                                return;
                            }
                            else
                            {
                                param.Clear();
                                qry = @"UPDATE wanranty_monitors SET wanranty_id = @warranty, issue_date = @datenow  WHERE id = @id";
                                param.Add(new SqlParameter("@warranty", SqlDbType.NVarChar) { Value = warrantyId });
                                param.Add(new SqlParameter("@datenow", SqlDbType.DateTime) { Value = DateTime.Now.ToLocalTime() });
                                param.Add(new SqlParameter("@id", SqlDbType.NVarChar) { Value = recNo });
                                isInsert = false;
                            }
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("fail get data: " + ex.Message,
                                "Get Data Failure", MessageBoxButtons.OK,
                                MessageBoxIcon.Hand,
                                MessageBoxDefaultButton.Button1);
                        }
                    }

                    try
                    {
                        int aff = ExecuteNonQuery(qry, param);

                        // RECORD SAVED COMPLETELY
                        if (aff > 0)
                        {
                            list = RefreshGrid(doNo, item);
                            dgv_mWarranty.DataSource = list;
                            int nextRowAvailable = Convert.ToInt32(list.Where(r => r.recNo == null).Select(r => r.lineNum).FirstOrDefault());
                            nextRowAvailable = nextRowAvailable == 0 ? list.Count - 1 : nextRowAvailable - 1;
                            int currWarrantyID = Convert.ToInt32(list.Where(r => r.recNo == null).Select(r => r.lineNum).FirstOrDefault()) - 1;
                            if (isInsert)
                                cbx_item.Items[idx] = boxItem[idx].AdjustAssembled(true);
                            tmpWarrantyID = string.Empty;
                            //* when saved warranty at last record qty of any item
                            if (currWarrantyID < 0)
                            {
                                int idxDO = cbx_selDO.SelectedIndex;
                                cbx_selDO.Items[idxDO] = boxDO[idxDO].AdjustAssembled(true);
                                cbx_selDO.SelectedIndex = idxDO;
                            }
                            cbx_item.SelectedIndex = idx;

                            if (currWarrantyID < 0)
                            {
                                AdjustStatusPickingJob(doNo, true);
                                tbx_txtScan.Text = dgv_mWarranty[Convert.ToInt32(nextRowAvailable), 1].ToString();
                            }
                            else
                            {
                                tbx_txtScan.Text = string.Empty;
                            }
                            dgv_mWarranty.CurrentRowIndex = Convert.ToInt32(nextRowAvailable);
                        }
                        else
                        {
                            MessageBox.Show("Save data error.",
                                "Manage Data Process",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Hand,
                                MessageBoxDefaultButton.Button1);
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("fail save data: " + ex.Message.ToString(),
                               "Save Data Failure", MessageBoxButtons.OK,
                               MessageBoxIcon.Hand,
                               MessageBoxDefaultButton.Button1);
                    }
                }
                else if (state.Equals(CONFIRM_DELETE))
                {
                    if (!string.IsNullOrEmpty(recNo))
                    {
                        qry = @"DELETE FROM wanranty_monitors WHERE id = @id";
                        param.Add(new SqlParameter("@id", SqlDbType.NVarChar) { Value = recNo });

                        try
                        {
                            int aff = ExecuteNonQuery(qry, param);
                            if (aff > 0)
                            {
                                list = RefreshGrid(doNo, item);
                                dgv_mWarranty.DataSource = list;
                                //cbx_item.Items[idx] = boxItem[idx].AdjustAssembled(false);
                                //cbx_item.SelectedIndex = idx;
                                long prevRowAvailable = (lineNum - 1 < 0 ? 0 : lineNum - 1);
                                int currWarrantyID = Convert.ToInt32(list.Where(r => r.recNo == null).Select(r => r.lineNum).FirstOrDefault()) - 1;
                                tmpWarrantyID = string.Empty;
                                cbx_item.Items[idx] = boxItem[idx].AdjustAssembled(false);
                                //12
                                //9
                                if (currWarrantyID == list.Count - 1)
                                {
                                    int idxDO = cbx_selDO.SelectedIndex;
                                    cbx_selDO.Items[idxDO] = boxDO[idxDO].AdjustAssembled(false);
                                    cbx_selDO.SelectedIndex = idxDO;
                                }
                                cbx_item.SelectedIndex = idx;
                                if (currWarrantyID == 0)
                                {
                                    tbx_txtScan.Text = string.Empty;
                                }
                                else
                                {
                                    tbx_txtScan.Text = dgv_mWarranty[Convert.ToInt32(prevRowAvailable), 1].ToString();
                                }
                                dgv_mWarranty.CurrentRowIndex = Convert.ToInt32(prevRowAvailable);

                                AdjustStatusPickingJob(doNo, false);
                            }
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("fail delete data: " + ex.Message,
                               "Delete Data Failure", MessageBoxButtons.OK,
                               MessageBoxIcon.Hand,
                               MessageBoxDefaultButton.Button1);
                        }
                    }
                }
            }
            var a = btn_confirm.Enabled;
            var b = btn_confirm.BackColor.ToString();
            var c = btn_confirm.Text;
            tbx_txtScan.TextChanged += tbx_txtScan_TextChanged;
            //* FORCED CALL text changed when done save/delete
            tbx_txtScan_TextChanged(null, null);
            tbx_txtScan.Focus();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            ServedPickingJob();
        }

        private void ServedPickingJob()
        {
            boxDO.Clear();
            cbx_selDO.Items.Clear();
            //cbx_selDO.Items.Add(genrerateLabelIndexing("DO Number"));
            boxDO.Add(new DO(genrerateLabelIndexing("DO Number"), -1, -1, -1, -1));
            cbx_selDO.Items.Add(boxDO[0].GetAssembledText());
            //            string qry = @"
            //DROP TABLE IF EXISTS #tmp
            //DECLARE @TIME_NOW DATETIME = GETDATE()
            //SELECT DISTINCT j.do_no
            //, COUNT(CASE WHEN (m.req_qty > 0 
            //        AND ISNULL(m.pick_qty, 0) = m.req_qty
            //        AND m.status = 1) 
            //        THEN m.item  
            //        END) [complt_item]
            //, CASE WHEN j.status=1 
            //    THEN 'DONE' 
            //    ELSE CONCAT(CAST(COUNT(CASE WHEN (m.req_qty > 0 
            //                        AND (ISNULL(m.pick_qty, 0) = m.req_qty) 
            //                        AND m.status = 1) 
            //                        THEN m.item 
            //                        END) AS VARCHAR), '/', CAST(COUNT(m.item) AS VARCHAR), ' items')
            //    END [completed_scanned]
            //INTO #tmp
            //FROM picking_jobs j
            //INNER JOIN picking_matls m
            //ON m.do_no = j.do_no
            //INNER JOIN master_items i
            //ON m.item = i.item
            //AND i.warranty_period IS NOT NULL
            //WHERE CONVERT(DATETIME, DATEADD(day, +1, ISNULL(modified_date, @TIME_NOW))) >= @TIME_NOW
            //GROUP BY j.do_no, j.status
            //SELECT do_no, completed_scanned
            //FROM #tmp
            //WHERE complt_item > 0";

            //            string qry = @"
            //DROP TABLE IF EXISTS #tmp
            //DECLARE @TIME_NOW DATETIME = GETDATE()
            //SELECT DISTINCT j.do_no
            //, COUNT(CASE WHEN (m.req_qty > 0 
            //        AND ISNULL(m.pick_qty, 0) = m.req_qty
            //        AND m.status = 1) 
            //        THEN m.item  
            //        END) [compt_item]
            //, COUNT(m.item) [req_item]
            //, ( SELECT CAST(SUM(m.req_qty) AS INT)) [all_req_qty]
            //, ( SELECT COUNT(do_no) 
            //    FROM wanranty_monitors wm
            //    WHERE wm.do_no = j.do_no) [all_compt_qty]
            //INTO #tmp
            //FROM picking_jobs j
            //INNER JOIN picking_matls m
            //ON m.do_no = j.do_no COLLATE SQL_Latin1_General_CP1_CS_AS
            //AND (m.status = 1
            //     AND m.req_qty = m.pick_qty)
            //INNER JOIN master_items i
            //ON m.item = i.item COLLATE SQL_Latin1_General_CP1_CS_AS
            //AND ( i.warranty_period IS NOT NULL OR i.warranty_period = 0)
            //WHERE CONVERT(DATETIME, DATEADD(day, +1, ISNULL(modified_date, @TIME_NOW))) >= @TIME_NOW
            //GROUP BY j.do_no, j.status
            //SELECT *
            //FROM #tmp
            //WHERE compt_item > 0";

            string qry = @"
DROP TABLE IF EXISTS #tmp
DECLARE @TIME_NOW DATETIME = GETDATE()
SELECT DISTINCT j.do_no
, COUNT(CASE WHEN (m.req_qty > 0 
        AND ISNULL(m.pick_qty, 0) = m.req_qty
        AND m.status = 1) 
        THEN m.item  
        END) [compt_item]
, COUNT(m.item) [req_item]
, ( SELECT CAST(SUM(m.req_qty) AS INT)) [all_req_qty]
, ( SELECT COUNT(do_no) 
    FROM wanranty_monitors wm
    WHERE wm.do_no = j.do_no) [all_compt_qty]
INTO #tmp
FROM picking_jobs j
INNER JOIN picking_matls m
ON m.do_no = j.do_no COLLATE SQL_Latin1_General_CP1_CS_AS
INNER JOIN master_items i
ON m.item = i.item COLLATE SQL_Latin1_General_CP1_CS_AS
AND ISNULL(i.warranty_period, 0) <> 0
WHERE CONVERT(DATETIME, DATEADD(day, +1, ISNULL(modified_date, @TIME_NOW))) >= @TIME_NOW
GROUP BY j.do_no, j.status
SELECT *
FROM #tmp
WHERE compt_item > 0";
            List<SqlParameter> param = new List<SqlParameter>();
            DataTable raw = ExecuteSelectQuery(qry, param);

            // attach
            int idx = 0;
            foreach (DataRow row in raw.Rows)
            {
                boxDO.Add(new DO(
                    row[0].ToString(),
                    int.Parse(row[1].ToString()),
                    int.Parse(row[2].ToString()),
                    int.Parse(row[3].ToString()),
                    int.Parse(row[4].ToString())
                    ));
                cbx_selDO.Items.Add(boxDO[++idx].GetAssembledText());
            }
            //foreach (DO doj in boxDO)
            //{
            //    if (string.IsNullOrEmpty(doj.status_text))
            //        cbx_selDO.Items.Add(doj.DOcode);
            //    else
            //        cbx_selDO.Items.Add(doj.DOcode + " (" + doj.status_text + ")");
            //}

            cbx_selDO.SelectedIndex = 0;
        }

        private void btn_exit_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Are you sure to quit program?",
                "Quit Program",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question,
                MessageBoxDefaultButton.Button1) == DialogResult.Yes)
            {
                Application.Exit();
            }
        }

        private void btn_refresh_Click(object sender, EventArgs e)
        {
            btn_confirm.Text = CONFIRM_SAVE;
            btn_confirm.BackColor = Color.Green;
            btn_confirm.Enabled = false;
            ServedPickingJob();
        }

        private void btn_clear_Click(object sender, EventArgs e)
        {
            clearText = true;
            tbx_txtScan.Text = string.Empty;
            int idx = dgv_mWarranty.CurrentRowIndex;
            if (idx != -1)
            {
                var warrantyId = dgv_mWarranty[idx, 1];
                if (warrantyId != null)
                {
                    btn_confirm.Text = CONFIRM_DELETE;
                    btn_confirm.BackColor = Color.Crimson;
                    btn_confirm.Enabled = true;
                }
            }
            dgv_mWarranty.Invalidate();
            tbx_txtScan.Focus();
        }

        private void cbx_selDO_SelectedIndexChanged(object sender, EventArgs e)
        {
            //string selDO = cbx_selDO.SelectedItem.ToString();
            string selDO = boxDO[cbx_selDO.SelectedIndex].DOcode;
            //cbx_item.Items.Add(genrerateLabelIndexing("Item"));

            boxItem.Clear();
            cbx_item.Items.Clear();
            boxItem.Add(new Item(genrerateLabelIndexing("Item"), -1, -1));
            cbx_item.Items.Add(boxItem[0].item);

            if (!selDO.Contains(SELECT_LABEL))
            {
                string qry = @"
SELECT m.item
,( 
    SELECT COUNT(wm.item) [scanned_qty]
    FROM wanranty_monitors wm 
    WHERE m.do_no = wm.do_no
    AND m.item = wm.item
) [scanned_qty],
CAST(m.req_qty AS INT) [req_qty]
FROM picking_matls m
INNER JOIN master_items i
ON i.item = m.item COLLATE SQL_Latin1_General_CP1_CS_AS
WHERE m.do_no = @seldo COLLATE SQL_Latin1_General_CP1_CS_AS
AND (m.status = 1 AND m.req_qty = m.pick_qty)
AND m.pick_qty = m.req_qty
AND ISNULL(i.warranty_period, 0) <> 0";
                List<SqlParameter> param = new List<SqlParameter>();
                param.Add(new SqlParameter("@seldo", SqlDbType.NVarChar) { Value = selDO });
                DataTable raw = ExecuteSelectQuery(qry, param);

                //box.Clear();
                //box.Add(SELECT_LABEL);
                //foreach (DataRow row in dt)
                //{
                //    box.Add(row[0].ToString());
                //    cbx_item.Items.Add(string.Concat(row[0], " (", Convert.ToInt32(row[1]), " Qty)"));
                //}
                cbx_item.Items.Clear();
                boxItem.Clear();
                boxItem.Add(new Item(genrerateLabelIndexing("Item"), -1, -1));
                //cbx_item.Items.Add(boxItem[0].item);
                int idx = 0;
                cbx_item.Items.Add(boxItem[0].item);
                foreach (DataRow row in raw.Rows)
                {
                    boxItem.Add(new Item(row[0].ToString(), int.Parse(row[1].ToString()), int.Parse(row[2].ToString())));
                    cbx_item.Items.Add(boxItem[++idx].GetAssembledText());
                }
                //foreach (Item i in boxItem)
                //{
                //    if (i.req_qty < 0)
                //        cbx_item.Items.Add(i.item);
                //    else
                //        cbx_item.Items.Add(i.item + " " + i.req_qty);

                //}
            }
            else
            {
                tbx_txtScan.Text = string.Empty;
                dgv_mWarranty.DataSource = null;
                btn_confirm.Enabled = false;
                btn_clear.Enabled = false;
                btn_confirm.BackColor = Color.Green;
                btn_confirm.Text = CONFIRM_SAVE;
            }
            cbx_item.SelectedIndex = 0;
        }

        public DataTable ExecuteSelectQuery(string query, List<SqlParameter> parameters)
        {
            DataTable dataTable = new DataTable();
            try
            {
                using (SqlConnection connection = new SqlConnection(SQL_CONNECTION))
                {
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        if (parameters.Count() > 0)
                        {
                            foreach (SqlParameter sqlp in parameters)
                            {
                                command.Parameters.Add(sqlp);
                            }
                        }

                        connection.Open();

                        using (SqlDataAdapter adapter = new SqlDataAdapter(command))
                        {
                            adapter.Fill(dataTable);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("error: " + ex.Message);
            }
            var a = dataTable.Rows.Count;
            return dataTable;
        }

        public int ExecuteNonQuery(string query, List<SqlParameter> parameters)
        {
            int rowsAffected = -1;
            try
            {
                using (SqlConnection connection = new SqlConnection(SQL_CONNECTION))
                {
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {

                        if (parameters.Count() > 0)
                        {
                            foreach (SqlParameter sqlp in parameters)
                            {
                                command.Parameters.Add(sqlp);
                            }
                        }

                        connection.Open();
                        rowsAffected = command.ExecuteNonQuery();

                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("error: " + ex.Message);
                rowsAffected = -1;
            }

            return rowsAffected;
        }

        private void cbx_item_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbx_item.Items.Count > 1)
            {
                //string selDO = cbx_selDO.SelectedItem.ToString()
                //string item = box[cbx_item.SelectedIndex];
                string selDO = boxDO[cbx_selDO.SelectedIndex].DOcode;
                string item = boxItem[cbx_item.SelectedIndex].item;
                if (!item.Contains(SELECT_LABEL))
                {
                    list = RefreshGrid(selDO, item);
                    dgv_mWarranty.DataSource = list;
                    dgv_mWarranty.CurrentRowIndex = 0;

                    //* INIT clear enabling
                    int idx = dgv_mWarranty.CurrentRowIndex;
                    if (idx != -1)
                    {
                        bool checkedItem = dgv_mWarranty[idx, 1] == null;
                        if (checkedItem)
                        {
                            btn_confirm.Text = CONFIRM_SAVE;
                            btn_confirm.BackColor = Color.Green;
                            btn_confirm.Enabled = false;
                            btn_clear.Enabled = false;
                        }
                        else
                        {
                            btn_confirm.Enabled = true;
                            btn_clear.Enabled = true;
                        }
                    }

                    dgv_mWarranty_Click(null, null);
                    int dgvWidth = dgv_mWarranty.Width - 50;

                    DataGridTableStyle tableStyle = new DataGridTableStyle();
                    tableStyle.MappingName = list.GetType().Name;

                    DataGridColumnStyle lnColstyle = new DataGridTextBoxColumn();
                    lnColstyle.MappingName = "lineNum";
                    lnColstyle.HeaderText = "No.";
                    lnColstyle.Width = (int)(dgvWidth * 0.2);
                    tableStyle.GridColumnStyles.Add(lnColstyle);

                    DataGridColumnStyle wdColstyle = new DataGridTextBoxColumn();
                    wdColstyle.MappingName = "warrantyID";
                    wdColstyle.HeaderText = "Warranty ID";
                    wdColstyle.NullText = string.Empty;
                    wdColstyle.Width = (int)(dgvWidth * 0.8) + 34;
                    tableStyle.GridColumnStyles.Add(wdColstyle);

                    DataGridColumnStyle recNo = new DataGridTextBoxColumn();
                    recNo.MappingName = "recNo";
                    recNo.HeaderText = string.Empty;
                    recNo.NullText = string.Empty;
                    recNo.Width = 0;
                    tableStyle.GridColumnStyles.Add(recNo);

                    dgv_mWarranty.TableStyles.Clear();
                    dgv_mWarranty.TableStyles.Add(tableStyle);
                    dgv_mWarranty.SelectionBackColor = Color.Red;
                }
                else
                {
                    dgv_mWarranty.DataSource = null;
                    tbx_txtScan.Text = string.Empty;
                    btn_confirm.Enabled = false;
                    btn_clear.Enabled = false;
                    btn_confirm.BackColor = Color.Green;
                    btn_confirm.Text = CONFIRM_SAVE;
                }
            }
            else
            {

            }
        }

        private void DrawCellText(Graphics g, Rectangle bounds, string text)
        {
            using (SolidBrush brush = new SolidBrush(Color.White))
            using (Font font = new Font("Tahoma", 24, FontStyle.Regular))
            {
                g.DrawString(text, font, brush, bounds.X + 2, bounds.Y + 2);
            }

        }

        private void dgv_mWarranty_Paint(object sender, PaintEventArgs e)
        {
            if (dgv_mWarranty.CurrentRowIndex > -1)
            {
                for (int i = 0; i < list.Count; i++)
                {
                    Rectangle rowBounds = dgv_mWarranty.GetCellBounds(i, 0);
                    Rectangle rowBounds2 = dgv_mWarranty.GetCellBounds(i, 1);
                    DataGridTableStyle tableStyle = dgv_mWarranty.TableStyles[0];

                    if ((dgv_mWarranty.CurrentRowIndex == i) && (dgv_mWarranty.CurrentRowIndex != -1))
                    {
                        e.Graphics.FillRectangle(new SolidBrush(Color.Red), rowBounds);
                        e.Graphics.FillRectangle(new SolidBrush(Color.Red), rowBounds2);

                        if (dgv_mWarranty[i, 0] != null)
                        {
                            string cellText = dgv_mWarranty[i, 0].ToString();
                            DrawCellText(e.Graphics, rowBounds, cellText);

                        }

                        if (dgv_mWarranty[i, 1] != null)
                        {
                            string cellText2 = dgv_mWarranty[i, 1].ToString();
                            DrawCellText(e.Graphics, rowBounds2, cellText2);
                        }
                    }
                    else
                    {

                    }
                }
            }
        }

        private void tbx_txtScan_GotFocus(object sender, EventArgs e)
        {
            string text = string.Empty;
            if (dgv_mWarranty.CurrentRowIndex != -1)
            {
                if (dgv_mWarranty[dgv_mWarranty.CurrentRowIndex, 1] != null)
                {
                    text = dgv_mWarranty[dgv_mWarranty.CurrentRowIndex, 1].ToString().Trim();
                }
            }

            if (clearText)
            {
                text = string.Empty;
                clearText = false;
            }

            tbx_txtScan.Text = text;
            tbx_txtScan.Select(tbx_txtScan.Text.Length, 0);
        }

        private void dgv_mWarranty_Click(object sender, EventArgs e)
        {
            btn_refresh.Focus(); //* temp clear focus
            tbx_txtScan.Focus();
            //tbx_txtScan.Select(tbx_txtScan.Text.Length, 0);
            dgv_mWarranty.Invalidate();
        }

        private void tbx_txtScan_KeyDown(object sender, KeyEventArgs e)
        {
            int currentRow = dgv_mWarranty.CurrentRowIndex;
            if (currentRow > -1)
            {
                var key = e.KeyValue;
                switch (key)
                {
                    //case 8: // CLR
                    //    var a = tbx_txtScan.Text;
                    //    if ((tbx_txtScan.Text.Trim().Length - 1) <= 0)
                    //    {
                    //        btn_confirm.Text = CONFIRM_DELETE;
                    //        btn_confirm.BackColor = Color.Crimson;
                    //    }
                    //    break; 
                    case 13: // ENT
                        var warrantyValue = dgv_mWarranty[dgv_mWarranty.CurrentRowIndex, 1];
                        if (warrantyValue == null && string.IsNullOrEmpty(tbx_txtScan.Text.Trim()))
                        {
                            MessageBox.Show("WarrantyID can not be null.",
                            "Manage Data Process",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Hand,
                            MessageBoxDefaultButton.Button1);
                            tbx_txtScan.Focus();
                            return;
                        }
                        tmpWarrantyID = tbx_txtScan.Text.Trim();
                        btn_confirm_Click(null, null);
                        break;
                    case 38: // UP
                        if (currentRow != 0)
                        {
                            dgv_mWarranty.CurrentRowIndex -= 1;
                            dgv_mWarranty_Click(null, null);
                        }
                        break;
                    case 40: // DOWN
                        if (currentRow < list.Count - 1)
                        {
                            dgv_mWarranty.CurrentRowIndex += 1;
                            dgv_mWarranty_Click(null, null);
                        }
                        break;
                    case 153: // SCAN
                        tbx_txtScan.Text = string.Empty;
                        break;
                }
            }
        }

        private void dgv_mWarranty_CurrentCellChanged(object sender, EventArgs e)
        {
            btn_confirm.Text = CONFIRM_SAVE;
            btn_confirm.BackColor = Color.Green;
            btn_confirm.Enabled = dgv_mWarranty[dgv_mWarranty.CurrentRowIndex, 1] != null;
            btn_clear.Enabled = dgv_mWarranty[dgv_mWarranty.CurrentRowIndex, 1] != null;
        }

        private void tbx_txtScan_TextChanged(object sender, EventArgs e)
        {
            var len = tbx_txtScan.Text.Trim().Length;
            int idx = dgv_mWarranty.CurrentRowIndex;
            int warrantyRecords = Convert.ToInt32(list.Where(r => r.recNo == null).Select(r => r.lineNum).FirstOrDefault());
            if (idx != -1)
            {
                bool isNewAddItem = dgv_mWarranty[idx, 1] == null;
                if (isNewAddItem)
                {
                    if (len == 0)
                    {
                        btn_confirm.BackColor = Color.Green;
                        btn_confirm.Enabled = false;
                        btn_confirm.Text = CONFIRM_SAVE;

                        btn_clear.Enabled = false;
                    }
                    else
                    {
                        btn_confirm.BackColor = Color.Green;
                        btn_confirm.Enabled = true;
                        btn_confirm.Text = CONFIRM_SAVE;

                        btn_clear.Enabled = true;
                    }
                }
                else
                {
                    if (len == 0)
                    {
                        btn_confirm.BackColor = Color.Crimson;
                        btn_confirm.Enabled = true;
                        btn_confirm.Text = CONFIRM_DELETE;

                        btn_clear.Enabled = false;
                    }
                    else
                    {
                        btn_confirm.BackColor = Color.Green;
                        btn_confirm.Enabled = true;
                        btn_confirm.Text = CONFIRM_SAVE;

                        btn_clear.Enabled = true;
                    }
                }
            }
        }
    }
}
class Warranty
{
    public long lineNum { get; set; }
    public string warrantyID { get; set; }
    public int? recNo { get; set; }
    public Warranty(long l, string w, int? recNo)
    {
        lineNum = l;
        warrantyID = w;
        this.recNo = recNo;
    }
}

class DO
{
    public string DOcode { get; set; }
    //* split
    public int all_req_qty { get; set; }
    public int all_scanned_qty { get; set; }
    private const string ALL_QTY_SCANNED = "DONE";
    public int completed_item { get; set; }
    public int requested_item { get; set; }
    public string status_text { get; set; }
    private string assembled_text { get; set; }
    public DO(string code, int c, int r, int ac, int ar)
    {
        DOcode = code;
        completed_item = c;
        requested_item = r;
        all_scanned_qty = ac;
        all_req_qty = ar;
        assembled_text = string.Empty;
    }
    public string GetAssembledText()
    {
        if ((this.completed_item > 0 && this.completed_item == this.requested_item)
            && (this.all_req_qty > 0 && this.all_req_qty == this.all_scanned_qty))
        {
            this.assembled_text = this.DOcode + " " + ALL_QTY_SCANNED;
        }
        else
        {
            this.assembled_text = this.DOcode + (this.completed_item < 0 && this.requested_item < 0 ? string.Empty : " (" +
            this.completed_item.ToString() + "/" +
            this.requested_item.ToString() + " item)");
        }
        return this.assembled_text;
    }

    public string AdjustAssembled(bool increase)
    {
        if (increase)
        {
            this.all_scanned_qty++;
        }
        else
        {
            this.all_scanned_qty--;
        }

        return this.GetAssembledText();

        //if (increase)
        //{
        //    //if (this.completed_item + 1 <= this.completed_item)
        //    //    this.completed_item++;
        //    if ((this.completed_item > 0 && this.completed_item == this.requested_item)
        //    && (this.all_req_qty > 0 && this.all_req_qty == this.all_scanned_qty))
        //    {
        //        this.assembled_text = this.DOcode + " " + ALL_QTY_SCANNED;
        //    }
        //}
        //else
        //{
        //    this.assembled_text = this.DOcode + (this.completed_item < 0 && this.requested_item < 0 ? string.Empty : " (" +
        //    this.completed_item.ToString() + "/" +
        //    this.requested_item.ToString() + " item)");
        //}
        //return this.assembled_text;
    }
}

class Item
{
    public string item { get; set; }
    public int req_qty { get; set; }
    public int scanned_qty { get; set; }
    private string assembled_text { get; set; }
    public Item(string i, int sq, int rq)
    {
        item = i;
        scanned_qty = sq;
        req_qty = rq;
        assembled_text = string.Empty;
    }
    public void setAssembledText()
    {

    }
    public string GetAssembledText()
    {
        if (this.req_qty < 0 && this.scanned_qty < 0)
        {
            this.assembled_text = this.item;
        }
        else
        {
            this.assembled_text = this.item + " (" +
            this.scanned_qty + "/" +
            this.req_qty + " Qty)";
        }
        return this.assembled_text;
    }
    public string AdjustAssembled(bool increase)
    {
        if (increase)
            this.scanned_qty++;
        else
            this.scanned_qty--;
        return this.GetAssembledText();
    }
}