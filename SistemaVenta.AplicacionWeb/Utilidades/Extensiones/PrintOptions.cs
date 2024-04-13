using System.Collections;
using System.Drawing.Printing;
using System.Drawing;
using System.Diagnostics;
using System.Diagnostics.Metrics;
using System;
using static System.Net.Mime.MediaTypeNames;
using System.Runtime.CompilerServices;

namespace nextadvisordotnet.AppWeb.Utilidades.Extensiones
{
    public class PrintOptions
    {
        #region Variables
        private ArrayList titleLines = new ArrayList();
        private ArrayList headerLines = new ArrayList();
        private ArrayList subHeaderLines = new ArrayList();
        private ArrayList items = new ArrayList();
        private ArrayList totalLines = new ArrayList();
        private ArrayList footerLines = new ArrayList();
        private string delimitador = "&&"; //Delimitador de filas (detalle/totales)
        private int count; //Contador de filas o renglones
        private int maxLengthLarge = 38; //Cantidad maxima de caracteres
        private int maxLengthLargeDetail = 0; //Cantidad maxima de caracteres detalle items
        private int maxCharDescription = 35; //Cantidad maxima de caracteres descripcion
        private float x_ml = 4; //Altura y dimension de cada renglon
        private float topMargin = 2f; //Margen Izquierdo
        private Font font = new Font("Consolas", 9, FontStyle.Regular); //Fuente principal
        private Font fontBold = new Font("Consolas", 10, FontStyle.Bold); //Fuente negrita      
        private Font fontSmall = new Font("Consolas", 7, FontStyle.Regular); //Fuente detalle items
        private SolidBrush myBrush = new SolidBrush(Color.Black); //Color fuente
        private Graphics graf;   //grafico
        #endregion

        /// <summary>
        /// Agrega las filas a imprimir en el ticket segun el tipo
        /// </summary>
        /// <param name="line">texto</param>
        /// <param name="type">tipo de fila a imprimir</param>
        public void AddRowLine(string line, TypeRow type)
        {
            switch (type)
            {
                case TypeRow.Title:
                    titleLines.Add(line);
                    break;
                case TypeRow.Header:
                    headerLines.Add(line);
                    break;
                case TypeRow.SubHeader:
                    subHeaderLines.Add(line);
                    break;  
                case TypeRow.Detail:
                    items.Add(line);
                    break;
                case TypeRow.Total:
                    totalLines.Add(line);
                    break;
                case TypeRow.Footer:
                    footerLines.Add(line);
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// Inicializa las variables de impresion
        /// </summary>
        /// <param name="impresora"></param>
        public void PrintTicket(string impresora)
        {
            maxLengthLargeDetail = maxLengthLarge + 11;
            PrintDocument printDocument = new PrintDocument();
            //PrinterSettings ps = new PrinterSettings(); //Imprimir a archivo
            //printDocument.PrinterSettings = ps; //Imprimir a archivo
            printDocument.PrinterSettings.PrinterName = impresora; //Imprimir Directo a dispositivo
            printDocument.PrintPage += new PrintPageEventHandler(event_PrintPage);
            printDocument.Print();
        }

        /// <summary>
        /// LLena de espacios segun el tipo maximo del ticket
        /// </summary>
        /// <param name="lenght">longitud del texto</param>
        /// <returns></returns>
        private string space_Fill(int lenght)
        {
            string text = "";
            int num = maxLengthLarge - lenght;
            for (int i = 0; i < num; i++)
                text += " ";

            return text;
        }

        /// <summary>
        /// LLena de espacios segun el tipo maximo del ticket
        /// </summary>
        /// <param name="lenght">longitud del texto</param>
        /// <returns></returns>
        private string space_FillDet(int lenght)
        {
            string text = "";
            int num = maxLengthLargeDetail - lenght;
            for (int i = 0; i < num; i++)
                text += " ";

            return text;
        }

        /// <summary>
        /// Obtiene el margen vertical para dibujar la linea de texto
        /// </summary>
        /// <returns></returns>
        private float y_pos()
        {
            return topMargin + ((float)count * fontSmall.GetHeight(graf));
        }

        /// <summary>
        /// Salto de linea en el plano grafico
        /// </summary>
        private void lineBreak()
        {
            graf.DrawString("", font, myBrush, x_ml, y_pos(), new StringFormat());
            count++;
        }

        /// <summary>
        /// Obtiene el texto centrado usando espacios a la izq
        /// </summary>
        /// <param name="text">texto</param>
        /// <param name="isBold">si es negrita</param>
        /// <returns></returns>
        private string centerString(string text, bool isBold = false)
        {
            int spacesLeft = ((maxLengthLarge - (isBold? 4 : 0)) - text.Length) / 2;
            return text.PadLeft(text.Length + spacesLeft);
        }

        /// <summary>
        /// Dibuja un string personalizado segun el tamaño maximo del texto
        /// </summary>
        /// <param name="text"></param>
        /// <param name="starIndex"></param>
        /// <param name="length"></param>
        /// <param name="fontCustom"></param>
        /// <param name="centered"></param>
        /// <param name="isBold"></param>
        private void drawStringCustom(string text, int starIndex, int length, Font fontCustom, bool centered, bool isBold = false)
        {
            text = text.Substring(starIndex, length);
            if(centered) text = centerString(text, isBold);
            graf.DrawString(text, fontCustom, myBrush, x_ml, y_pos(), new StringFormat());
            count++;
        }
        
        /// <summary>
        /// Evento de dibujo del ticket
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void event_PrintPage(object sender, PrintPageEventArgs e)
        {
            e.Graphics!.PageUnit = GraphicsUnit.Millimeter;
            this.graf = e.Graphics;
            this.JoinSections();
        }

        /// <summary>
        /// Reune todas las lineas de texto a dibujar en el grafico por secciones
        /// </summary>
        private void JoinSections()
        {
            string lineText;
            #region Title
            foreach (string title in titleLines)
            {
                lineText = "" + title + "";
                if (title.Length > maxLengthLarge)
                {
                    int num = 0;
                    for (int num2 = title.Length; num2 > maxLengthLarge; num2 -= maxLengthLarge)
                    {
                        this.drawStringCustom(title, num, maxLengthLarge, fontBold, true, true);
                        num += maxLengthLarge;
                    }
                    this.drawStringCustom(title, num, title.Length - num, fontBold, true, true);
                }
                else
                {
                    lineText = centerString(lineText,true);
                    graf.DrawString(lineText, this.fontBold, myBrush, x_ml, y_pos(), new StringFormat());
                    count++;
                }
            }
            #endregion

            #region Header
            foreach (string headerLine in headerLines)
            {
                lineText = headerLine;
                if (headerLine.Length > maxLengthLarge)
                {
                    int num = 0;
                    for (int num2 = headerLine.Length; num2 > maxLengthLarge; num2 -= maxLengthLarge)
                    {
                        this.drawStringCustom(headerLine, num, maxLengthLarge, font, true, false);
                        num += maxLengthLarge;
                    }
                    this.drawStringCustom(headerLine, num, headerLine.Length - num, font, true, false);
                }
                else
                {
                    lineText = centerString(lineText);
                    graf.DrawString(lineText, font, myBrush, x_ml, y_pos(), new StringFormat());
                    count++;
                }
            }
            lineBreak();
            #endregion

            #region SubHeader
            foreach (string subHeaderLine in subHeaderLines)
            {
                lineText = subHeaderLine;
                if (subHeaderLine.Length > maxLengthLarge)
                {
                    int num = 0;
                    for (int num2 = subHeaderLine.Length; num2 > maxLengthLarge; num2 -= maxLengthLarge)
                    {
                        this.drawStringCustom(subHeaderLine, num, maxLengthLarge, font, false, false);
                        num += maxLengthLarge;
                    }
                    this.drawStringCustom(subHeaderLine, num, subHeaderLine.Length - num, font, false, false);
                }
                else
                {
                    graf.DrawString(lineText, font, myBrush, x_ml, y_pos(), new StringFormat());
                    count++;
                }
            }
            lineBreak();
            #endregion

            #region Items Detail
            graf.DrawString("CANT   DESCRIPCION               TOTAL", font, myBrush, x_ml, y_pos(), new StringFormat());
            count++;
            lineBreak();
            foreach (string item in items)
            {
                string[] array = item.Split(delimitador);
                string cant = array[0]; //Cantidad;
                string nameProd = array[1]; //Nonbre Producto;
                string total = Convert.ToDecimal(array[2]).ToString("N0"); //Precio Total;

                //Cantidad
                graf.DrawString(cant, fontSmall, myBrush, x_ml, y_pos(), new StringFormat());

                //Precio total
                lineText = space_FillDet(total.Length) + total;
                graf.DrawString(lineText, fontSmall, myBrush, x_ml, y_pos(), new StringFormat());

                //Descripcion
                if (nameProd.Length > maxCharDescription)
                {
                    int num = 0;
                    for (int num2 = nameProd.Length; num2 > maxCharDescription; num2 -= maxCharDescription)
                    {
                        graf.DrawString("      " + nameProd.Substring(num, maxCharDescription), fontSmall, myBrush, x_ml, y_pos(), new StringFormat());
                        count++;
                        num += maxCharDescription;
                    }
                    graf.DrawString("      " + nameProd.Substring(num, nameProd.Length - num), fontSmall, myBrush, x_ml, y_pos(), new StringFormat());
                    count++;
                }
                else
                {
                    graf.DrawString("      " + nameProd, fontSmall, myBrush, x_ml, y_pos(), new StringFormat());
                    count++;
                }
            }
            lineBreak();
            #endregion

            #region Totales
            foreach (string t in totalLines)
            {
                if (t.Contains("-"))
                {
                    graf.DrawString(t.PadLeft(maxLengthLarge,'-'), font, myBrush, x_ml, y_pos(), new StringFormat());
                    count++;
                    continue;
                }
                string[] array = t.Split(delimitador);
                string desc = array[0];
                string total = Convert.ToDecimal(array[1]).ToString("N0");

                //Descripcion
                graf.DrawString(desc, font, myBrush, x_ml, y_pos(), new StringFormat());

                //Precio
                lineText = space_Fill(total.Length) + total;
                graf.DrawString(lineText, font, myBrush, x_ml, y_pos(), new StringFormat());

                count++;
            }
            lineBreak();
            #endregion

            #region Footer
            foreach (string footerLine in footerLines)
            {
                lineText = footerLine;
                if (lineText.Length > maxLengthLarge)
                {
                    int num = 0;
                    for (int num2 = lineText.Length; num2 > maxLengthLarge; num2 -= maxLengthLarge)
                    {
                        this.drawStringCustom(footerLine, num, maxLengthLarge, font, true, false);
                        num += maxLengthLarge;
                    }
                    this.drawStringCustom(footerLine, num, footerLine.Length - num, font, true, false);
                }
                else
                {
                    lineText = centerString(lineText);
                    graf.DrawString(lineText, font, myBrush, x_ml, y_pos(), new StringFormat());
                    count++;
                }
            }
            lineBreak(); 
            #endregion
        }        
    }

    public enum TypeRow
    {
        Title,
        Header,
        SubHeader,
        Detail,
        Total,
        Footer
    }

}
