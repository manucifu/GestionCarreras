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
using System.Data.OleDb;

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
            cli_alta.Visible = false;
            cli_consulta.Visible = false;
            cli_modificar.Visible = false;

        }
        private void button1_Click(object sender, EventArgs e)
        {
            Boolean numeroVerdad = false;
            try
            {
                float.Parse(textBox1.Text);
                numeroVerdad = true;
            }
            catch (Exception err)
            {
                MessageBox.Show("Tiene que ser un número");
            }

            if (numeroVerdad)
            {
                InsertarDorsal(Int32.Parse(textBox1.Text));
                //MessageBox.Show(corredores.Count.ToString());
                generarCod(corredores.Count);

            }
        }

        PictureBox[] pict;
        private void generarCod(int vueltas)
        {
            //int vueltas = Int32.Parse(textBox2.Text) - Int32.Parse(textBox1.Text);
            dor_barras.AutoScroll = true;

            PictureBox[] picturebox1 = new PictureBox[vueltas];
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
                }
                else
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

        String ultimo = "";
        private void textBox3_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                ultimo = textBox3.Text;
                textBox4.Text = ultimo;
                textBox3.Clear();
            }
        }

        private void consultaToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            paneles();
            dor_consulta.Visible = true;
            textBox3.Focus();
        }

        String valor;
        private void button3_Click(object sender, EventArgs e)
        {
            valor = textBox7.Text;
            OleDbCommand miCmd;
            OleDbConnection miCnx;
            String nombre = textBox5.Text;
            String dni = textBox7.Text;
            String apellido = textBox6.Text;
            //String fecha_nac = dateTimePicker1.Value;
            String fecha = dateTimePicker1.Value.ToString("dd/MM/yyyy");

            if (apellido != "" && nombre != "" && VerificarNIF(valor))
            {
                try
                {
                    String conexion = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=|DataDirectory|\\corredores.mdb;Persist Security Info=True";
                    string sentencia = "INSERT INTO corredores (nombre, apellidos, dni , fecha_nac ) VALUES('" + nombre + "','" + apellido + "','" + dni + "','" + fecha + "')";
                    miCnx = new OleDbConnection(conexion);
                    miCmd = new OleDbCommand(sentencia, miCnx);
                    miCnx.Open();
                    miCmd.ExecuteNonQuery();

                    MessageBox.Show("Cliente insertado");

                    miCnx.Close();
                }
                catch (OleDbException err2)
                {
                    MessageBox.Show("Error con la base de datos" + err2);
                }
            }
            else
            {
                if (nombre == "")
                    MessageBox.Show("error Al introducir el nombre");
                if (apellido == "")
                    MessageBox.Show("Error al introducir el apellido");
                if (!VerificarNIF(valor))
                    MessageBox.Show("DNI incorrecto");
            }

        }

        public Boolean VerificarNIF(String valor)
        {
            if (valor == "")
            {
                return false;
            }else
            {
                String aux = null;
                valor = valor.ToUpper();
                // ponemos la letra en mayúscula
                aux = valor.Substring(0, valor.Length - 1);
                // quitamos la letra del NIF
                if (aux.Length >= 7 && this.CadenaEsNumero(aux))
                    aux = this.CalculaNIF(aux); // calculamos la letra del NIF para comparar con la que tenemos
                else
                    return false;
                // comparamos las letras
                return (valor == aux);
            }
        }

        public Boolean CadenaEsNumero(String pp)
        {
            try
            {
                int entero = Int32.Parse(pp);
                return true;
            }
            catch (Exception err)
            {
                return false;
            }
        }

        private String CalculaNIF(String strA)
        {
            const String cCADENA = "TRWAGMYFPDXBNJZSQVHLCKE";
            const String cNUMEROS = "0123456789";

            Int32 a = 0;
            Int32 b = 0;
            Int32 c = 0;
            Int32 NIF = 0;
            StringBuilder sb = new StringBuilder();

            strA = strA.Trim();
            if (strA.Length == 0) return "";

            // Dejar sólo los números
            for (int i = 0; i <= strA.Length - 1; i++)
                if (cNUMEROS.IndexOf(strA[i]) > -1) sb.Append(strA[i]);

            strA = sb.ToString();
            a = 0;
            NIF = Convert.ToInt32(strA);
            do
            {
                b = Convert.ToInt32((NIF / 24));
                c = NIF - (24 * b);
                a = a + c;
                NIF = b;
            } while (b != 0);

            b = Convert.ToInt32((a / 23));
            c = a - (23 * b);
            return strA.ToString() + cCADENA.Substring(c, 1);
        }

        private void altaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            paneles();
            cli_alta.Visible = true;
        }

        private void consultaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            paneles();
            cli_consulta.Visible = true;
            this.corredoresTableAdapter.Fill(this.corredoresDataSet.corredores);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // TODO: esta línea de código carga datos en la tabla 'corredoresDataSet.corredores' Puede moverla o quitarla según sea necesario.
            this.corredoresTableAdapter.Fill(this.corredoresDataSet.corredores);

        }

        private void modificaciónToolStripMenuItem_Click(object sender, EventArgs e)
        {
            paneles();
            cli_modificar.Visible = true;
            ComboDoble();
        }

        public void ComboDoble()
        {
            string conexion = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=|DataDirectory|\\corredores.mdb;Persist Security Info=True";
            OleDbConnection miCnx = new OleDbConnection(conexion);
            string sentencia = "SELECT Id, nombre + ' ' + apellidos as completo FROM corredores ORDER BY nombre";
            OleDbCommand miCmd = new OleDbCommand(sentencia, miCnx);
            miCnx.Open();
            OleDbDataAdapter adapter = new OleDbDataAdapter();
            DataSet ds = new DataSet();

            adapter.SelectCommand = miCmd;
            adapter.Fill(ds);
            adapter.Dispose();
            miCmd.Dispose();
            comboBox1.DataSource = ds.Tables[0];
            comboBox1.ValueMember = "Id";
            comboBox1.DisplayMember = "completo";

            miCnx.Close();
        }

        private void comboBox1_SelectionChangeCommitted(object sender, EventArgs e)
        {
            OleDbCommand miCmd;
            OleDbConnection miCnx;
            OleDbDataReader miLector;
            try
            {
                String conexion = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=|DataDirectory|\\corredores.mdb;Persist Security Info=True";
                string sentencia = "SELECT nombre, apellidos, dni, fecha_nac from corredores WHERE Id = " + comboBox1.SelectedValue + "";
                miCnx = new OleDbConnection(conexion);
                miCmd = new OleDbCommand(sentencia, miCnx);
                miCnx.Open();
                miLector = miCmd.ExecuteReader();
                miLector.Read();

                String nombre = miLector.GetString(0);
                String apellidos = miLector.GetString(1);
                String dni = miLector.GetString(2);
                String fecha = miLector.GetString(3);

                //separo el dia, mes y año de la fecha para a continuacion introducirla en el datetimePicker
                int dia = Int32.Parse(fecha.Substring(0, 2));
                int mes = Int32.Parse(fecha.Substring(3, 2));
                int anio = Int32.Parse(fecha.Substring(6, 4));

                textBox9.Text = nombre;
                textBox10.Text = apellidos;
                textBox8.Text = dni;
                dateTimePicker2.Value = new DateTime(anio, mes, dia);

                miCnx.Close();
                miLector.Close();

            }
            catch (OleDbException err3)
            {
                MessageBox.Show("Error con la base de datos " + err3);
            }
        }

        private void button3_Click_1(object sender, EventArgs e)
        {
            OleDbCommand miCmd;
            OleDbConnection miCnx;
            OleDbDataReader miLector;
            String nombre = textBox9.Text;
            String apellido = textBox10.Text;
            String dni = textBox8.Text;

            if (apellido != "" && nombre != "" && VerificarNIF(dni))
            {
                try
                {
                    String conexion = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=|DataDirectory|\\corredores.mdb;Persist Security Info=True";
                    string sentencia = "UPDATE corredores SET nombre='" + nombre + "', apellidos='" + apellido + "', dni='" + dni + "' , fecha_nac='" + dateTimePicker2.Value.ToShortDateString() + "' WHERE Id=" + comboBox1.SelectedValue + "";
                    miCnx = new OleDbConnection(conexion);
                    miCmd = new OleDbCommand(sentencia, miCnx);
                    miCnx.Open();
                    miCmd.ExecuteNonQuery();
                    miCnx.Close();

                    ComboDoble();
                }
                catch (OleDbException err3)
                {
                    MessageBox.Show("Error con la base de datos " + err3);
                }
                MessageBox.Show("Corredor modificado");
            }
            else
            {
                if (apellido == "")
                    MessageBox.Show("Apellido no valido");
                if (nombre == "")
                    MessageBox.Show("Nombre no valido");
                if (!VerificarNIF(dni))
                    MessageBox.Show("DNI no valido");
            }
        }

        List<Corredor> corredores = new List<Corredor>();
        public void InsertarDorsal(int numeroDorsal)
        {
            corredores.Clear();
            ConsultaDorsal();
            int dorNum = numeroDorsal;
            foreach (Corredor corr in corredores)
            {
                OleDbCommand miCmd;
                OleDbConnection miCnx;
                try
                {
                    String conexion = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=|DataDirectory|\\corredores.mdb;Persist Security Info=True";
                    string sentencia = "UPDATE corredores SET dorsal='" + dorNum + "' WHERE Id=" + Int32.Parse(corr.getId()) + "";
                    miCnx = new OleDbConnection(conexion);
                    miCmd = new OleDbCommand(sentencia, miCnx);
                    miCnx.Open();
                    miCmd.ExecuteNonQuery();
                    miCnx.Close();

                    ComboDoble();
                }
                catch (OleDbException err3)
                {
                    MessageBox.Show("Error con la base de datos " + err3);
                }
                //MessageBox.Show("Corredor modificado");

                dorNum++;
            }
            
        }

        public void ConsultaDorsal()
        {
            OleDbCommand miCmd;
            OleDbConnection miCnx;
            OleDbDataReader miLector;
            try
            {
                String conexion = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=|DataDirectory|\\corredores.mdb;Persist Security Info=True";
                string sentencia = "SELECT Id, nombre, apellidos, dni, fecha_nac from corredores";
                miCnx = new OleDbConnection(conexion);
                miCmd = new OleDbCommand(sentencia, miCnx);
                miCnx.Open();
                miLector = miCmd.ExecuteReader();
                
                while (miLector.Read())
                {
                    Corredor corr = new Corredor();
                    corr.setId((miLector.GetInt32(0)).ToString());
                    corr.setNombre(miLector.GetString(1));
                    corr.setApellidos(miLector.GetString(2));
                    corr.setDni(miLector.GetString(3));
                    corr.setFecha_nac(miLector.GetString(4));
                    corredores.Add(corr);
                } 
                miCnx.Close();
                miLector.Close();

            }
            catch (OleDbException err3)
            {
                MessageBox.Show("Error con la base de datos " + err3);
            }
        }

        private void textBox4_TextChanged(object sender, EventArgs e)
        {
            OleDbCommand miCmd;
            OleDbConnection miCnx;
            OleDbDataReader miLector;
            try
            {
                String conexion = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=|DataDirectory|\\corredores.mdb;Persist Security Info=True";
                string sentencia = "SELECT nombre, apellidos, dni, fecha_nac from corredores WHERE dorsal = '" + textBox4.Text + "'";
                miCnx = new OleDbConnection(conexion);
                miCmd = new OleDbCommand(sentencia, miCnx);
                miCnx.Open();
                miLector = miCmd.ExecuteReader();
                miLector.Read();

                label19.Text = miLector.GetString(0);
                label20.Text = miLector.GetString(1);
                label21.Text = miLector.GetString(3);
                label22.Text = miLector.GetString(2);

                miCnx.Close();
                miLector.Close();

            }
            catch (Exception err3)
            {
                MessageBox.Show("Error, no existe registro");
                Console.Write(err3);
            }
        }
    }
}
