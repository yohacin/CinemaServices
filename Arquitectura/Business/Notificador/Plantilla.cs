using Data;
using Entities.Notificador;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

#region Abreviaturas

using ENO = Entities.Notificador;

#endregion Abreviaturas

namespace Business.Notificador
{
    public class Plantilla : Base
    {
        public Plantilla(IApplicationDbContext ApplicationDbContext) : base(ApplicationDbContext)
        {
        }

        public bool Guardar(ENO.Plantilla ePlantilla)
        {
            using (var oTrans = this._DBcontext.Database.BeginTransaction())
            {
                try
                {
                    this._DBcontext.Plantilla.Add(ePlantilla);
                    this._DBcontext.SaveChanges();

                    int indexQuery = 1;
                    foreach (ENO.Query query in ePlantilla.oLstQueries)
                    {
                        query.id_plantilla = ePlantilla.id;
                        query.id_query = indexQuery;
                        this._DBcontext.Query.Add(query);
                        indexQuery++;
                    }

                    foreach (ENO.Adjunto adjunto in ePlantilla.oLstAdjutos)
                    {
                        adjunto.id = 0;
                        adjunto.id_plantilla = ePlantilla.id;
                        this._DBcontext.Adjunto.Add(adjunto);
                    }

                    this._DBcontext.SaveChanges();

                    oTrans.Commit();
                    return true;
                }
                catch (Exception ex)
                {
                    oTrans.Rollback();
                    //this._log.Error(ex);
                    throw ex;
                }
            }
        }

        public ENO.Plantilla Buscar(long id)
        {
            try
            {
                ENO.Plantilla entity = this._DBcontext.Plantilla.FirstOrDefault(p => p.id == id);

                if (entity != null)
                {
                    Query bQuery = new Query(this._DBcontext);
                    entity.oLstQueries = bQuery.ListarByPlantilla(entity.id);
                    Adjunto bAdjunto = new Adjunto(this._DBcontext);
                    entity.oLstAdjutos = bAdjunto.ListarByPlantilla(entity.id);
                }

                return entity;
            }
            catch (Exception ex)
            {
                //this._log.Error(ex);
                throw ex;
            }
        }

        public bool Modificar(ENO.Plantilla ePlantilla)
        {
            using (var oTrans = this._DBcontext.Database.BeginTransaction())
            {
                try
                {
                    this._DBcontext.Plantilla.Update(ePlantilla);
                    this._DBcontext.SaveChanges();

                    this._DBcontext.Query.RemoveRange(this._DBcontext.Query.Where(q => q.id_plantilla == ePlantilla.id));
                    this._DBcontext.SaveChanges();

                    int indexQuery = 1;
                    foreach (ENO.Query query in ePlantilla.oLstQueries)
                    {
                        query.id = 0;
                        query.id_plantilla = ePlantilla.id;
                        query.id_query = indexQuery;
                        this._DBcontext.Query.Add(query);
                        indexQuery++;
                    }

                    string sDeleteString = @"DELETE FROM notificador.adjunto WHERE id_plantilla = @p_idPlantilla";
                    this._DBcontext.Database.ExecuteSqlRaw(sDeleteString, new NpgsqlParameter("@p_idPlantilla", ePlantilla.id));

                    foreach (ENO.Adjunto adjunto in ePlantilla.oLstAdjutos)
                    {
                        adjunto.id = 0;
                        adjunto.id_plantilla = ePlantilla.id;
                        this._DBcontext.Adjunto.Add(adjunto);
                    }

                    this._DBcontext.SaveChanges();

                    oTrans.Commit();
                    return true;
                }
                catch (Exception ex)
                {
                    oTrans.Rollback();
                    //this._log.Error(ex);
                    throw ex;
                }
            }
        }

        public List<ENO.Plantilla> ListarByEmpresa(long id_empresa)
        {
            try
            {
                return this._DBcontext.Plantilla.Where(
                    p => p.id_empresa == id_empresa &&
                    p.marca_eliminado == 0
                ).ToList();
            }
            catch (Exception ex)
            {
                //this._log.Error(ex);
                throw ex;
            }
        }

        public List<ENO.Plantilla> ListarByEmpresaTipo(long id_empresa, ENO.TipoPlantilla tipo_plantilla)
        {
            try
            {
                return this._DBcontext.Plantilla.Where(
                    p => p.id_empresa == id_empresa &&
                    p.tipo == tipo_plantilla.StringValue() &&
                    p.marca_eliminado == 0
                ).ToList();
            }
            catch (Exception ex)
            {
                //this._log.Error(ex);
                throw ex;
            }
        }

        public List<ENO.Plantilla> ListarByCampana(long codigo_campana)
        {
            try
            {
                string sQuery = $@"
                                    SELECT p.*
                                    FROM notificador.plantilla p
                                    INNER JOIN notificador.campana_plantilla cp ON cp.id_plantilla = p.id
                                    WHERE cp.codigo_campana = {codigo_campana}";

                return this._DBcontext.Plantilla.FromSqlRaw(sQuery).ToList();
            }
            catch (Exception ex)
            {
                //this._log.Error(ex);
                throw ex;
            }
        }

        public bool Eliminar(long id)
        {
            using (var oTrans = this._DBcontext.Database.BeginTransaction())
            {
                try
                {
                    ENO.Plantilla ePlantilla = this.Buscar(id);
                    if (ePlantilla == null)
                    {
                        throw new Exception("Plantilla Inexistente!!...");
                    }
                    ePlantilla.marca_eliminado = 9;
                    this._DBcontext.Entry(ePlantilla).State = EntityState.Modified;
                    this._DBcontext.SaveChanges();

                    oTrans.Commit();
                    return true;
                }
                catch (Exception ex)
                {
                    oTrans.Rollback();
                    //this._log.Error(ex);
                    throw ex;
                }
            }
        }

        public List<ENO.Query> ObtenerCamposQuery(List<ENO.Query> queries)
        {
            try
            {
                //List<NpgsqlParameter> oParamList = new List<NpgsqlParameter>();
                int indexQuery = 1;
                foreach (ENO.Query query in queries)
                {
                    query.id_query = indexQuery;
                    query.queryResults = _DBcontext.CollectionFromSql(query.contenido).ToList<Object>();
                    indexQuery++;
                }

                return queries;
            }
            catch (Exception ex)
            {
                //this._log.Error(ex);
                throw ex;
            }
        }

        private bool IsArrayJson(String valor)
        {
            try
            {
                JArray jsonItems = JsonConvert.DeserializeObject(valor) as JArray;
                var rowItems = jsonItems.OfType<JObject>().ToList();
            }
            catch (Exception ex)
            {
                return false;
            }

            return true;
        }

        public String ObtenerPreviewQuery(String query)
        {
            try
            {
                //List<NpgsqlParameter> oParamList = new List<NpgsqlParameter>();
                List<Object> queryResult = _DBcontext.CollectionFromSql(query).ToList<Object>();

                if (queryResult == null || queryResult.Count == 0)
                {
                    return "";
                }

                String attributeTemplate = @"<p style='font-size: 20px;'><strong style='font-style: italic;'>{0}: </strong>{1}</p>";

                dynamic firstResult = queryResult[0];

                StringBuilder result = new StringBuilder();
                foreach (KeyValuePair<String, Object> property in firstResult)
                {
                    String clave = property.Key.ToString();
                    String valor = property.Value.ToString();

                    if (IsArrayJson(valor))
                    {
                        result.AppendFormat(attributeTemplate, clave, "");
                        result.Append(ProcessJsonArray(valor));
                        result.Append("<br>");
                    }
                    else
                    {
                        result.AppendFormat(attributeTemplate, clave, valor);
                    }
                }

                return result.ToString();
            }
            catch (Exception ex)
            {
                //this._log.Error(ex);
                throw ex;
            }
        }

        private bool IsMultitable(String sArrayJson)
        {
            try
            {
                JArray jsonItems = JsonConvert.DeserializeObject(sArrayJson) as JArray;
                var rowItems = jsonItems.OfType<JObject>().ToList();

                /*  PRE VALIDATION MULTI TABLE */
                JObject firstPrevalidation = rowItems.FirstOrDefault();
                foreach (JProperty itemProperty in firstPrevalidation.Properties())
                {
                    if (itemProperty.Value.Type == JTokenType.Array)
                    {
                        return true;
                    }
                }
                return false;
            }
            catch (Exception ex)
            {
                //this._log.Error("Error al validar multitabla: ", ex);
                return false;
            }
        }

        public String ProcessJsonArray(String sArrayJson)
        {
            if (!IsMultitable(sArrayJson))
            {
                return ArrayJsonToHtmlTable(sArrayJson);
            }

            String informationPropertyTemplate = @"<br><div style='position:relative;width: 100%;background: #008bf9;border: solid 1px black;border-bottom: none;color: white;text-align: center;font-weight: bold;text-transform: uppercase;'>{0}</div>";

            StringBuilder multiTableResult = new StringBuilder();

            JArray jsonItems = JsonConvert.DeserializeObject(sArrayJson) as JArray;
            var rowItems = jsonItems.OfType<JObject>().ToList();

            foreach (JObject item in rowItems)
            {
                List<JProperty> properties = item.Properties().ToList();
                JProperty informationProperty = properties[0];
                JProperty detailsProperty = properties[1];

                StringBuilder containerTable = new StringBuilder();

                multiTableResult.Append("<br>");
                multiTableResult.Append(ArrayJsonToHtmlTable(detailsProperty.Value.ToString(), informationProperty.Value.ToString()));
            }

            return multiTableResult.ToString();
        }

        public String ArrayJsonToHtmlTable(String sArrayJson, String customHeaderContent = null)
        {
            String tableTemplate = @"<table border='0' cellpadding='0' width='100%' cellspacing='0' style='border: 1px solid black;'>{0}</table>";
            String tableHeaderTemplate = @"<tr><th colspan='{0}' scope='colgroup' style='padding: 5px 14px; border: 1px solid black;background:#0082ef;color:white;font-weight:bold;text-transform:uppercase;'>{1}</th></tr>";
            String tableHeadTemplate = @"<thead style='background:#0082ef;color:white;font-weight:bold;text-transform:uppercase;'>{0}</thead>";
            String tableBodyTemplate = @"<tbody>{0}</tbody>";
            String tableFooterTemplate = @"<tfoot>{0}</tfoot>";
            String rowTemplate = @"<tr>{0}</tr>";
            String colHeaderTemplate = @"<th style='padding: 5px 14px; border: 1px solid black;background:blue;color:white;font-weight:bold;text-transform:uppercase;'>{0}</th>";
            String colTemplate = @"<td style='padding: 5px 14px; border: 1px solid black;'>{0}</td>";

            StringBuilder table = new StringBuilder();
            StringBuilder customHeader = new StringBuilder();
            StringBuilder tableHeader = new StringBuilder();
            StringBuilder tableBody = new StringBuilder();
            StringBuilder tableFooter = new StringBuilder();
            StringBuilder rows = new StringBuilder();
            StringBuilder cols = new StringBuilder();

            JArray jsonItems = JsonConvert.DeserializeObject(sArrayJson) as JArray;

            if (jsonItems == null)
            {
                return "El campo no es una tabla válida!";
            }

            var rowItems = jsonItems.OfType<JObject>().ToList();

            /* TABLE HEADER */

            JObject firstItem = rowItems.FirstOrDefault();

            List<String> headerNames = firstItem.Properties().Select(p => p.Name).ToList();
            int nroCols = 0;
            foreach (String headerName in headerNames)
            {
                cols.AppendFormat(colHeaderTemplate, headerName);
                nroCols++;
            }
            if (customHeaderContent != null)
            {
                tableHeader.AppendFormat(tableHeaderTemplate, nroCols, customHeaderContent);
            }
            rows.AppendFormat(rowTemplate, cols.ToString());
            tableHeader.AppendFormat(tableHeadTemplate, rows.ToString());

            /* TABLE BODY*/
            rows = new StringBuilder();

            foreach (JObject item in rowItems)
            {
                cols = new StringBuilder();
                foreach (JProperty itemProperty in item.Properties())
                {
                    cols.AppendFormat(colTemplate, itemProperty.Value.ToString());
                }
                rows.AppendFormat(rowTemplate, cols.ToString());
            }
            tableBody.AppendFormat(tableBodyTemplate, rows.ToString());

            /* TABLE FOOTER */
            rows = new StringBuilder();
            cols = new StringBuilder();
            // Footer code

            table.AppendFormat(tableTemplate, tableHeader.ToString() + tableBody.ToString() + tableFooter.ToString());

            return table.ToString();
        }

        public string ReemplazarValoresTemplate(String plantilla, List<String> queries)
        {
            //List<NpgsqlParameter> oParamList = new List<NpgsqlParameter>();
            int indexQuery = 1;
            foreach (String query in queries)
            {
                List<Object> queryResult = _DBcontext.CollectionFromSql(query).ToList<Object>();

                if (queryResult == null || queryResult.Count == 0)
                {
                    return "";
                }

                dynamic firstResult = queryResult[0];

                foreach (KeyValuePair<String, Object> property in firstResult)
                {
                    String clave = property.Key.ToString();
                    String valor = property.Value.ToString();

                    //Regex regex = new Regex(@"{" + clave + Regex.Escape("|") + @"\d}");
                    Regex regex = new Regex(@"{" + Regex.Escape(clave) + Regex.Escape("|") + Regex.Escape("[") + @"[a-z]*" + Regex.Escape("]") + Regex.Escape("|") + "Q" + indexQuery + @"}");
                    String recuperado = regex.Match(plantilla).ToString();

                    if (recuperado == "")
                    {
                        continue;
                    }

                    String valores = recuperado.Substring(1, recuperado.Length - 2);

                    String valorFormateado = null;

                    String tipoParametro = valores.Split('|')[1];
                    //int tipoParametro = Int32.Parse(valores.Split('|')[1]);
                    switch (tipoParametro)
                    {
                        case "[texto]": // Texto
                            valorFormateado = valor;
                            break;

                        case "[numerico]": // Númerico
                            valorFormateado = valor;
                            break;

                        case "[fecha]": // Fecha
                            valorFormateado = valor;
                            break;

                        case "[tabla]": // Tabla
                            valorFormateado = ProcessJsonArray(valor);
                            break;

                        default:
                            valorFormateado = "";
                            break;
                    }

                    plantilla = regex.Replace(plantilla, valorFormateado);
                }
                indexQuery++;
            }

            return plantilla;
        }
    }
}