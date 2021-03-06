﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using DAL;
using BLL;
using System.Data;

namespace Junior_Santiago___Aplicada2___p2
{
    public partial class Formulario : System.Web.UI.Page
    {
      
        protected void Page_Load(object sender, EventArgs e)
        {
            DataTable dt = new DataTable();
             Articulos ar = new Articulos();
            if(!IsPostBack)
            {
                FecharTextBox.Text = DateTime.Now.ToString("dd/MM/yyyy");
                AddColumnas();
            }

            LlenarDrowList();

        }

        protected void AgregarButton_Click(object sender, EventArgs e)
        {
            try
            {
            DataTable dt = (DataTable)ViewState["Detalle"];
            DataRow row;
            Articulos ar = new Articulos();
                int Existencia = 0;
               
            ar.ObtenerDatosArticuloTex(ArticulosDropDownList.Text);

              
                  row = dt.NewRow();
                  row["ArticuloId"] = ar.ArticuloId;
                  row["Cantidad"] = CantidadTextBox0.Text;
                  row["Precio"] = ar.Precio;

                int.TryParse(CantidadTextBox0.Text, out Existencia);
                if(ar.Existencia < Existencia)
                {
                    Utilitarios.ShowToastr(this, "Quedan "+ar.Existencia +" "+ArticulosDropDownList.Text + " Existente ", "Mensaje", "error");
                }
                else
                {
                  dt.Rows.Add(row);
                  ViewState["Detalle"] = dt;               
                  ObtenerGridView();            
                  TotalTextBox.Text = Total().ToString();

                    ArticulosDropDownList.Items.Clear();
                    LlenarDrowList();

                  CantidadTextBox0.Text = "";
                }
                  
            
          

               
            }catch(Exception ex)
            {
                throw ex;
            }
          
            
        }
        public void ObtenerDatos(Ventas ven,Articulos ar)
        {
            
            ven.Fecha = FecharTextBox.Text;
            ven.Monto = Convert.ToSingle(TotalTextBox.Text);
            ven.VetaId = ConvertirId();
            int idAux = 0;

            foreach(GridViewRow row in DetalleGridView.Rows)
            {
                ven.AgregarArticulos(Convert.ToInt32(row.Cells[0].Text), Convert.ToInt32(row.Cells[1].Text), Convert.ToSingle(row.Cells[2].Text));
                idAux = Convert.ToInt32(row.Cells[1].Text);
                ar.ObtenerDatosArticulo(Convert.ToInt32(row.Cells[0].Text));
                ar.AgregarExistencia(Convert.ToInt32(row.Cells[0].Text),ar.Existencia, Convert.ToInt32(row.Cells[1].Text));
     
            }
          
        }
        public void ObtenerGridView()
        {
            DetalleGridView.DataSource = (DataTable)ViewState["Detalle"];
            DetalleGridView.DataBind();
        }
        public void AddColumnas()
        {
            DataTable dt = new DataTable();

            dt.Columns.AddRange(new DataColumn[3] { new DataColumn("ArticuloId"), new DataColumn("Cantidad"), new DataColumn("Precio") });
            ViewState["Detalle"] = dt;
        }
        public float Total()
        {
            float total = DetalleGridView.Rows.Cast<GridViewRow>().Sum(x => Convert.ToSingle(x.Cells[2].Text));
            return total;
        }
        public int TotalExistencia()
        {

               int total = DetalleGridView.Rows.Cast<GridViewRow>().Sum(x => Convert.ToInt32(x.Cells[1].Text));
               return total;
     
        }
        void LimpiarGrid()
        {

            DetalleGridView.DataSource = null;
            DetalleGridView.DataBind();
        }
        void Limpiar()
        {
            CantidadTextBox0.Text = "";
            TotalTextBox.Text = "";
            BuscarTextBox.Text = "";
            LimpiarGrid();
        }
        public void LLenarValor(Ventas ven)
        {
            DataTable dt = (DataTable)ViewState["Detalle"];
            TotalTextBox.Text = ven.Monto.ToString();
            FecharTextBox.Text = ven.Fecha.ToString();

            foreach(var item in ven.Detalle)
            {
                
                dt.Rows.Add(item.ArticuloId, item.Cantidad, item.Precio);
                ViewState["Detalle"] = dt;
                ObtenerGridView();
            }

        }
        public int ConvertirId()
        {
            int id = 0;
            int.TryParse(BuscarTextBox.Text, out id);

            return id;
        }
        public void LlenarDrowList()
        {
            DataTable dt = new DataTable();
            Articulos ar = new Articulos();

            dt = ar.Listar("*", "0=0", "ORDER BY Descripcion");
            for (int i = 0; i <= dt.Rows.Count - 1; i++)
                ArticulosDropDownList.Items.Add(Convert.ToString(ar.Listar("*", "0=0", "ORDER BY Descripcion").Rows[i]["Descripcion"]));
        }

        protected void NuevoButton_Click(object sender, EventArgs e)
        {
            Limpiar();
            LimpiarGrid();
        }

        protected void BuscarButton_Click(object sender, EventArgs e)
        {
            Ventas ven = new Ventas();
            
            if(string.IsNullOrWhiteSpace(BuscarTextBox.Text))
            {

            }
            else
            {
                if(ven.Buscar(ConvertirId()))
                {  
                    LLenarValor(ven);
                }
                else
                {
                    Utilitarios.ShowToastr(this, "Id Incorrecto", "Mensaje", "error");

                }
              
            }
        }

        protected void GuardarButton_Click(object sender, EventArgs e)
        {
            Ventas ven = new Ventas();
            Articulos ar = new Articulos();
           
            if (DetalleGridView.Rows.Count == 0)
            {
                Utilitarios.ShowToastr(this, "Error", "Mensaje", "error");
            }
            else
            {
                 ObtenerDatos(ven,ar);
                ar.AfectarExistencia();
                if (string.IsNullOrWhiteSpace(BuscarTextBox.Text))
                {
                    if (ven.Insertar())
                    {
                        
                    }
                }
                else
                {
                    if (ven.Actualizar())
                    {
                       
                    }
                }
                   Utilitarios.ShowToastr(this, "Guardado", "Mensaje", "success");
            }
              
         }
        
        protected void EliminarButton_Click(object sender, EventArgs e)
        {
            Ventas venta = new Ventas();
            int id = 0;
            int.TryParse(BuscarTextBox.Text, out id);
            venta.VetaId = id;
            if(venta.Eliminar())
            {
                Utilitarios.ShowToastr(this, "Eliminado", "Mensaje", "success");
                Limpiar();
            }
            
        }
    }
}