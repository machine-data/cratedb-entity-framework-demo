using System.Linq;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Windows.Forms;
using System;

namespace EFChartCrateDB
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, System.EventArgs e)
        {
            using (var db = new CrateDBContext())
            {
                // display all posts from CrateDB
                var query = (from s in db.Data
                            orderby s.ts ascending
                            group s by s.ts into g
                            select new {
                                ts = g.Key,
                                max_value = g.Average(s => s.value)
                            }).Take(50);


                foreach (var item in query)
                {
                    DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
                    dtDateTime = dtDateTime.AddMilliseconds(item.ts);

                    sensorChart.Series["temp"].Points.AddXY(dtDateTime.ToString("hh:mm:ss.fff"), item.max_value);
                }
            }

        }
    }

    [Table("doc.sensor_data")]
    public class SensorData
    {
        [Key]
        public string id { get; set; }
        public float value { get; set; }
        public long ts { get; set; }
    }

    public class CrateDBContext : DbContext
    {
        public CrateDBContext()
        {
            //Configuration.AutoDetectChangesEnabled = false;
            Database.SetInitializer<CrateDBContext>(null);
        }

        public DbSet<SensorData> Data { get; set; }
    }

}
