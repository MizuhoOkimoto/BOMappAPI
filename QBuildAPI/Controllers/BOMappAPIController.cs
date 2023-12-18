using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace QBuildAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BOMappAPIController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        public BOMappAPIController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpGet]
        [Route("GetData")]
        public JsonResult GetData()
        {
            string query = "SELECT bom.PARENT_NAME, bom.COMPONENT_NAME, bom.QUANTITY, part.PART_NUMBER, part.TITLE, part.TYPE, part.ITEM, part.MATERIAL "
                + "FROM bom LEFT JOIN part ON bom.COMPONENT_NAME = part.NAME;";
            DataTable table = new DataTable();
            string sqlDatasource = _configuration.GetConnectionString("API");
            using (SqlConnection myCon = new SqlConnection(sqlDatasource))
            {
                myCon.Open();
                using (SqlCommand myCommand = new SqlCommand(query, myCon))
                {
                    using (SqlDataReader myReader = myCommand.ExecuteReader())
                    {
                        table.Load(myReader);

                        // Add an ID column to the table if it does not exist
                        if (!table.Columns.Contains("ID"))
                        {
                            table.Columns.Add("ID", typeof(int)).SetOrdinal(0);
                            // Populate the ID column with a unique identifier
                            for (int i = 0; i < table.Rows.Count; i++)
                            {
                                table.Rows[i]["ID"] = i + 1;
                            }
                        }

                        myReader.Close();
                    }
                }
                myCon.Close();
            }
            return new JsonResult(table);
        }
    }
}
