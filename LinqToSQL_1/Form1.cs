using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LinqToSQL_1
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        NorthWindDataContext ctx;
        private void btnEkle_Click(object sender, EventArgs e)
        {
            ctx = new NorthWindDataContext();

            Product p = new Product();
            p.ProductName = txtProductName.Text;
            p.UnitPrice = nudFiyat.Value;
            p.UnitsInStock = Convert.ToInt16(nudStock.Value);
            p.CategoryID = (int)comboKategori.SelectedValue;
            p.SupplierID = (int)comboTedarikci.SelectedValue;

            ctx.Products.InsertOnSubmit(p);
            MessageBox.Show($"SubmitChanges oncesi ProductID = {p.ProductID}");
            ctx.SubmitChanges();
            MessageBox.Show($"SubmitChanges sonrasi ProductID = {p.ProductID}");

            ListeDoldur();
        }

        private void btnSil_Click(object sender, EventArgs e)
        {
            NorthWindDataContext ctx = new NorthWindDataContext();
            Product p = new Product();
            if (dataGridView1.CurrentRow == null)
                return;
            int urunId = (int)dataGridView1.CurrentRow.Cells["ProductID"].Value;
            p = ctx.Products.SingleOrDefault(urun => urun.ProductID == urunId);
            ctx.Products.DeleteOnSubmit(p);
            ctx.SubmitChanges();
            //refresh the grid
            //dataGridView1.DataSource = ctx.Products;
            ListeDoldur();
        }

        private void btnKaydet_Click(object sender, EventArgs e)
        {
            ctx = new NorthWindDataContext();

            Product p = ctx.Products.SingleOrDefault(urun => urun.ProductID == (int)txtProductName.Tag);

            p.ProductName = txtProductName.Text;
            p.UnitPrice = nudFiyat.Value;
            p.UnitsInStock = Convert.ToInt16(nudStock.Value);
            p.CategoryID = (int)comboKategori.SelectedValue;
            p.SupplierID = (int)comboTedarikci.SelectedValue;

            ctx.SubmitChanges(); // değişiklikleri ADO.Net koduna cevirerek veritabanina gonder
            //refresh the grid
            //dataGridViev1.DataSource = ctx.Products;
            ListeDoldur();
        }

        private void txtAra_TextChanged(object sender, EventArgs e)
        {
            ctx = new NorthWindDataContext();
            dataGridView1.DataSource = ctx.Products.Where(x => x.ProductName.Contains(txtAra.Text)); //Contains içeren anlamında textbox a yazılan string ifadenin içinde geçtiği sonuçları gösterir.
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            //YOL 1 Kısa yol , DatagridView'e products tablosunu aktarmak için
            //ama istediğimiz tüm kolonlardaki değerleri göremiyoruz.
            //(sadece alltaki iki satırla)
            //NorthWindDataContext ctx = new NorthWindDataContext();
            //dataGridView1.DataSource = ctx.Products;Yol 1(Kısa yol ama direk tabloyu döndürüyor.Farklı kolonlar görmek istersek ikinci yol denenmelidir!)

            //YOL 2
            ListeDoldur();

        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            DataGridViewRow r = dataGridView1.CurrentRow;
            txtProductName.Text = r.Cells["ProductName"].Value.ToString();
            nudFiyat.Value = Convert.ToDecimal(r.Cells["UnitPrice"].Value);
            nudStock.Value = Convert.ToDecimal(r.Cells["UnitsInStock"].Value);
            comboKategori.SelectedValue = (int)r.Cells["CategoryID"].Value;
            comboTedarikci.SelectedValue = (int)r.Cells["SupplierID"].Value;
            txtProductName.Tag = r.Cells["ProductID"].Value;
        }

        public void ListeDoldur()
        {
            ctx = new NorthWindDataContext();
            dataGridView1.DataSource = ctx.Products;

            comboKategori.DisplayMember = "CategoryName";
            comboKategori.ValueMember = "CategoryID";
            comboKategori.DataSource = ctx.Categories;

            comboTedarikci.DisplayMember = "CompanyName";
            comboTedarikci.ValueMember = "SupplierID";
            comboTedarikci.DataSource = ctx.Suppliers;

            var query = from p in ctx.Products
                        join s in ctx.Suppliers on p.SupplierID equals s.SupplierID
                        join c in ctx.Categories on p.CategoryID equals c.CategoryID
                        select new
                        {
                            p.ProductID,
                            p.ProductName,
                            p.UnitPrice,
                            p.UnitsInStock,
                            c.CategoryName,
                            c.CategoryID,
                            s.CompanyName,
                            s.SupplierID
                        };

            dataGridView1.DataSource = query;
            dataGridView1.Columns["CategoryID"].Visible = false;
            dataGridView1.Columns["SupplierID"].Visible = false;
        }
    }
}
