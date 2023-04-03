using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace lab03_demo
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        QuanLySinhVien qlsv;

        private string imagePath;
        #region phuong thuc bo tro 
        private SinhVien GetSinhVien()
        {
            SinhVien sv = new SinhVien();
            bool gt = true;
            List<string> cn = new List<string>();
            sv.MaSo = this.mktbMaSo.Text;
            sv.HoTen = this.txtHoTen.Text;
            sv.NgaySinh = this.dtpNgaySinh.Value;
            sv.DiaChi = this.txtDiaChi.Text;
            sv.Lop = this.cbLop.Text;
            sv.Hinh = this.txtHinh.Text;
            if (rbNu.Checked)
                gt = false;
            sv.GioiTinh = gt;
            for (int i = 0; i < this.cbChuyenNganh.Items.Count; i++)
                if (cbChuyenNganh.GetItemChecked(i))
                    cn.Add(cbChuyenNganh.Items[i].ToString());
            sv.ChuyenNganh = cn;
            return sv;
        }
        private SinhVien GetSinhVienLV(ListViewItem lvitem)
        {
            SinhVien sv = new SinhVien();
            sv.MaSo = lvitem.SubItems[0].Text;
            sv.HoTen = lvitem.SubItems[1].Text;
            sv.NgaySinh = DateTime.Parse(lvitem.SubItems[2].Text);
            sv.DiaChi = lvitem.SubItems[3].Text;
            sv.Lop = lvitem.SubItems[4].Text;
            sv.GioiTinh = false;
            if (lvitem.SubItems[5].Text == "Nam")
                sv.GioiTinh = true;
            List<string> cn = new List<string>();
            string[] s = lvitem.SubItems[6].Text.Split(',');
            foreach (string t in s)
                cn.Add(t);
            sv.ChuyenNganh = cn;
            sv.Hinh = lvitem.SubItems[7].Text;
            return sv;
        }
        private void ThietLapThongTin(SinhVien sv)
        {
            this.mktbMaSo.Text = sv.MaSo;
            this.txtHoTen.Text = sv.HoTen;
            this.dtpNgaySinh.Value = sv.NgaySinh;
            this.txtDiaChi.Text = sv.DiaChi;
            this.cbLop.Text = sv.Lop;
            this.txtHinh.Text = sv.Hinh;
            this.pbHinh.ImageLocation = sv.Hinh;
            if (sv.GioiTinh)
                this.rbNam.Checked = true;
            else
                this.rbNu.Checked = true;
            for (int i = 0; i < this.cbChuyenNganh.Items.Count; i++)
                this.cbChuyenNganh.SetItemChecked(i, false);
            foreach (string s in sv.ChuyenNganh)
            {
                for (int i = 0; i < this.cbChuyenNganh.Items.Count;
                i++)
                    if
                    (s.CompareTo(this.cbChuyenNganh.Items[i]) == 0)
                        this.cbChuyenNganh.SetItemChecked(i,
                        true);
            }
        }
        private void ThemSV(SinhVien sv)
        {
            ListViewItem lvitem = new ListViewItem(sv.MaSo);
            lvitem.SubItems.Add(sv.HoTen);
            lvitem.SubItems.Add(sv.NgaySinh.ToShortDateString());
            lvitem.SubItems.Add(sv.DiaChi);
            lvitem.SubItems.Add(sv.Lop);
            string gt = "Nữ";
            if (sv.GioiTinh)
                gt = "Nam";
            lvitem.SubItems.Add(gt);
            string cn = "";
            foreach (string s in sv.ChuyenNganh)
                cn += s + ",";
            cn = cn.Substring(0, cn.Length - 1);
            lvitem.SubItems.Add(cn);
            lvitem.SubItems.Add(sv.Hinh);
            this.lvSinhVien.Items.Add(lvitem);

        }
        private void LoadListView()
        {
            this.lvSinhVien.Items.Clear();
            foreach (SinhVien sv in qlsv.DanhSach)
            {
                ThemSV(sv);
            }
        }


        #endregion

        private void Form1_Load(object sender, EventArgs e)
        {
            qlsv = new QuanLySinhVien();
            qlsv.DocTuFile();
            LoadListView();
        }

        private void lvSinhVien_SelectedIndexChanged(object sender, EventArgs e)
        {
            int count = this.lvSinhVien.SelectedItems.Count;
            if (count > 0)
            {
                ListViewItem lvitem =
                this.lvSinhVien.SelectedItems[0];
                SinhVien sv = GetSinhVienLV(lvitem);
                ThietLapThongTin(sv);
            }
        }

        private void btnThem_Click(object sender, EventArgs e)
        {
            SinhVien sv = GetSinhVien();
            SinhVien kq = qlsv.Tim(sv.MaSo,
            delegate (object obj1, object obj2)
            {
                return (obj2 as
    SinhVien).MaSo.CompareTo(obj1.ToString());
            });
            if (kq != null)
                MessageBox.Show("Mã sinh viên đã tồn tại!", "Lỗi thêm dữ liệu", MessageBoxButtons.OK, MessageBoxIcon.Error);
            else
            {
                this.qlsv.Them(sv);
                this.LoadListView();
            }
        }

        private void btnThoat_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
        private int SoSanhTheoMa(object obj1, object obj2)
        {
            SinhVien sv = obj2 as SinhVien;
            return sv.MaSo.CompareTo(obj1);
        }

        private void btnXoa_Click(object sender, EventArgs e)
        {
            int count, i;
            ListViewItem lvitem;
            count = this.lvSinhVien.Items.Count - 1;
            for (i = count; i >= 0; i--)
            {
                lvitem = this.lvSinhVien.Items[i];
                if (lvitem.Checked)
                    qlsv.Xoa(lvitem.SubItems[0].Text, SoSanhTheoMa);
            }
            this.LoadListView();
            this.btndatlai.PerformClick();
        }

        private void btndatlai_Click(object sender, EventArgs e)
        {
            this.mktbMaSo.Text = "";
            this.txtHoTen.Text = "";
            this.dtpNgaySinh.Value = DateTime.Now;
            this.txtDiaChi.Text = "";
            this.cbLop.Text = this.cbLop.Items[0].ToString();
            this.txtHinh.Text = "";
            this.pbHinh.ImageLocation = "";
            this.rbNam.Checked = true;
            for (int i = 0; i < this.cbChuyenNganh.Items.Count - 1;
            i++)
                this.cbChuyenNganh.SetItemChecked(i, false);

        }

        private void btnSua_Click(object sender, EventArgs e)
        {
            SinhVien sv = GetSinhVien();
            bool kqsua;
            kqsua = qlsv.Sua(sv, sv.MaSo, SoSanhTheoMa);
            if (kqsua)
            {
                this.LoadListView();
            }

        }

        private void btnBrowse_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.InitialDirectory = "C:\\";
            openFileDialog.Filter = "Image Files (*.bmp;*.jpg;*.jpeg,*.png)|*.BMP;*.JPG;*.JPEG;*.PNG";
            openFileDialog.FilterIndex = 1;
            openFileDialog.RestoreDirectory = true;
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                txtHinh.Text = openFileDialog.FileName;
            }
            if (File.Exists(txtHinh.Text))
            {
                pbHinh.Image = Image.FromFile(txtHinh.Text);
            }
        }

        private void txtHinh_TextChanged(object sender, EventArgs e)
        {
            
        }

        private void groupBox1_Enter(object sender, EventArgs e)
        {

        }

        private void menuStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {

        }

        private void menuStrip1_ItemClicked_1(object sender, ToolStripItemClickedEventArgs e)
        {

        }

        private void mởFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.InitialDirectory = "C:\\";
            openFileDialog.Filter = "Image Files (*.bmp;*.jpg;*.jpeg,*.png)|*.BMP;*.JPG;*.JPEG;*.PNG";
            openFileDialog.FilterIndex = 1;
            openFileDialog.RestoreDirectory = true;
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                txtHinh.Text = openFileDialog.FileName;
            }
            if (File.Exists(txtHinh.Text))
            {
                pbHinh.Image = Image.FromFile(txtHinh.Text);
            }
        }

        private void thoátToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void pbHinh_Click(object sender, EventArgs e)
        {

        }

        private void thêmToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SinhVien sv = GetSinhVien();
            SinhVien kq = qlsv.Tim(sv.MaSo,
            delegate (object obj1, object obj2)
            {
                return (obj2 as
    SinhVien).MaSo.CompareTo(obj1.ToString());
            });
            if (kq != null)
                MessageBox.Show("Mã sinh viên đã tồn tại!", "Lỗi thêm dữ liệu", MessageBoxButtons.OK, MessageBoxIcon.Error);
            else
            {
                this.qlsv.Them(sv);
                this.LoadListView();
            }
        }

        private void xoáToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int count, i;
            ListViewItem lvitem;
            count = this.lvSinhVien.Items.Count - 1;
            for (i = count; i >= 0; i--)
            {
                lvitem = this.lvSinhVien.Items[i];
                if (lvitem.Checked)
                    qlsv.Xoa(lvitem.SubItems[0].Text, SoSanhTheoMa);
            }
            this.LoadListView();
            this.btndatlai.PerformClick();
        }

        private void sửaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SinhVien sv = GetSinhVien();
            bool kqsua;
            kqsua = qlsv.Sua(sv, sv.MaSo, SoSanhTheoMa);
            if (kqsua)
            {
                this.LoadListView();
            }
        }

        private void fontToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FontDialog fontDialog = new FontDialog();
            if (fontDialog.ShowDialog() == DialogResult.OK)
            {
                lvSinhVien.Font = fontDialog.Font;
            }
        }

        private void màuChữToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ColorDialog colordialog = new ColorDialog();
            if (colordialog.ShowDialog() == DialogResult.OK)
            {
                lvSinhVien.ForeColor = colordialog.Color;
            }
        }
    }
}
