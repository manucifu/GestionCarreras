using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using BarcodeLib;
using System.Drawing.Printing;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System.IO;
using System.Text.RegularExpressions;

namespace ProyectoCarreras
{
    public partial class Form1 : Form
    {
        
        public Form1()
        {
            InitializeComponent();
            paneles();
        }
        private void paneles()
        {
            dor_generar.Visible = false;
            dor_consulta.Visible = false;

        }
        private void button1_Click(object sender, EventArgs e)
        {
            //Boolean numeroVerdad = false;
            /*try
            {
                float.Parse(textBox1.Text);
                float.Parse(textBox2.Text);
                numeroVerdad = true;
            }
            catch (Exception err)
            {
                MessageBox.Show("Tiene que ser un número");
            }

            if (numeroVerdad)
            {*/
            generarCod();
            //}
        }

        PictureBox[] pict;
        private void generarCod()
        {
            int vueltas = Int32.Parse(textBox2.Text) - Int32.Parse(textBox1.Text);
            dor_barras.AutoScroll = true;

            PictureBox[]  picturebox1 = new PictureBox[vueltas];
            pict = new PictureBox[vueltas];
            int y = 0;
            int imprimir = Int32.Parse(textBox1.Text);
            Boolean horizontal = true;
            for (int index = 0; index < vueltas; index++)
            {
                BarcodeLib.Barcode codigo = new BarcodeLib.Barcode();
                codigo.IncludeLabel = true;
                picturebox1[index] = new PictureBox();
                pict[index] = new PictureBox(); //pickture box auxiliar para copiar los codigos generados
                dor_barras.Controls.Add(picturebox1[index]);
                // Following three lines set the images(picture boxes) locations
                if (index % 2 == 0)
                    y = y + 150; // 2 images per rows, first image will be at (20,150)
                // Para poner dos imagenes en la misma linea 
                if (horizontal)
                {
                    picturebox1[index].Location = new Point(0, y);
                    horizontal = false;
                }else
                {
                    picturebox1[index].Location = new Point(450, y);
                    horizontal = true;
                }

                picturebox1[index].Size = new Size(400, 100);
                picturebox1[index].Image = codigo.Encode(TYPE.CODE128, imprimir.ToString(), Color.Black, Color.White, 300, 100);

                pict[index].Size = new Size(150, 50);
                pict[index].Image = codigo.Encode(TYPE.CODE128, imprimir.ToString(), Color.Black, Color.White, 300, 100);
                imprimir++;
            }  
        }

        private void button2_Click(object sender, EventArgs e)
        {
            int vueltas = Int32.Parse(textBox2.Text) - Int32.Parse(textBox1.Text);
            SaveFileDialog svg = new SaveFileDialog();
            svg.ShowDialog();

            Document doc = new Document(iTextSharp.text.PageSize.LETTER, 10, 10, 42, 35);
            PdfWriter wri = PdfWriter.GetInstance(doc, new FileStream(svg.FileName + ".pdf", FileMode.Create));
            doc.Open();
            for (int i = 0; i < vueltas; i++)
            {
                iTextSharp.text.Image jpeg = iTextSharp.text.Image.GetInstance(pict[i].Image, System.Drawing.Imaging.ImageFormat.Jpeg);
                doc.Add(jpeg);
                Paragraph saltoDeLinea = new Paragraph("                                                                                                                                                                                                                                                                                                                                                                                   ");
                doc.Add(saltoDeLinea);
            }
            doc.Close();
        }

        private void altaToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            paneles();
            dor_generar.Visible = true;
        }

        String aux;
        Boolean bandera = true;
        private void textBox3_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                //bandera = true;
                /*aux = textBox3.Text;
                String aux = sender.ToString();
                aux.ToLower();
                String pablo = Regex.Replace(aux, "([^0-9])", "");
                textBox3.Text = "";*/
            }
        }

        private void consultaToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            paneles();
            dor_consulta.Visible = true;
            textBox3.Focus();
        }

        
        private void textBox3_TextChanged(object sender, EventArgs e)
        {
            if (bandera)
            {
                textBox4.Clear();
                bandera = false;
                aux = textBox3.Text;
                textBox3.Clear();
                
                
            }
            
            //aux = textBox3.Text;

        }

        private void textBox3_KeyUp(object sender, KeyEventArgs e)
        {

            if (e.KeyValue == (char)Keys.Enter)
            {
                textBox4.Text = "";
                string aux2 = textBox3.Text;       
                textBox4.Text = aux + aux2;
                //textBox3.Text = aux + textBox3.Text;
                bandera = true;
            }
        }
    }
}
