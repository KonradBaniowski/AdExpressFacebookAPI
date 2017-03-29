using Newtonsoft.Json;
using Oracle.DataAccess.Client;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OracleDataToJson
{
    public class RelationalFactory
    {

        private string connectionString = "Data Source=(DESCRIPTION =  (ADDRESS_LIST =  (ADDRESS = (PROTOCOL = TCP)(HOST = 172.17.236.126)(PORT = 1521))) (CONNECT_DATA = (SID = ADEXPR03)));User Id=dmussuma;Password=sandie5;";
        string projectDirectory = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.FullName;
        OracleDataReader dr;

        public string ExportHourDataToJsonFile()
        {
            ArrayList objs = new ArrayList();

            using (OracleConnection con = new OracleConnection(connectionString))
            {
                using (OracleCommand cmd = new OracleCommand())
                {
                    con.Open();
                    cmd.Connection = con;

                    cmd.CommandText = @"
                        select TRUNC(t.DATE_CREATION, 'HH24') AS HOURD, t.ID_LOGIN, LOGIN, count(*) As NB_CO
                        from WEBNAV02.TRACKING_ARCHIVE t, MAU01.LOGIN l
                        where t.id_login = l.id_login
                        AND ID_EVENT = 1
                        AND t.DATE_CREATION >= TO_DATE('01/01/2016', 'DD/MM/YYYY')
                        GROUP BY TRUNC(t.DATE_CREATION, 'HH24'), t.ID_LOGIN, LOGIN
                    ";

                    dr = cmd.ExecuteReader();

                    if (dr.HasRows)
                    {
                        while (dr.Read())
                        {
                            objs.Add(new
                            {
                                hour = Convert.ToInt64(Convert.ToDateTime(dr["HOURD"]).ToString("yyyyMMddHH")),
                                id_login = Convert.ToInt64(dr["ID_LOGIN"].ToString()),
                                login = dr["LOGIN"].ToString(),
                                connectionNumber = Convert.ToInt64(dr["NB_CO"].ToString()),

                            });

                        }
                    }
                    var jsonObj = JsonConvert.SerializeObject(objs);

                    File.WriteAllText(Path.Combine(projectDirectory, Path.Combine("output", "hourTracking.json")), jsonObj);
                }

            }

            return Path.Combine(projectDirectory, Path.Combine("output", "hourTracking.json"));
        }

        public string ExportDayDataToJsonFile()
        {
            ArrayList objs = new ArrayList();
            string jsonObj = string.Empty;

            using (OracleConnection con = new OracleConnection(connectionString))
            {
                using (OracleCommand cmd = new OracleCommand())
                {
                    con.Open();
                    cmd.Connection = con;

                    cmd.CommandText = @"
                        SELECT TRUNC(t.DATE_CREATION, 'DD') AS DAYD, t.ID_LOGIN, LOGIN, count(ID_TRACKING_ARCHIVE) AS NB_CO
                        FROM WEBNAV02.TRACKING_ARCHIVE t, MAU01.LOGIN l
                        WHERE t.id_login = l.id_login
                        AND ID_EVENT = 1
                        GROUP BY TRUNC(t.DATE_CREATION, 'DD'), t.ID_LOGIN, LOGIN
                    ";

                    dr = cmd.ExecuteReader();

                    if (dr.HasRows)
                    {
                        while (dr.Read())
                        {
                            objs.Add(new
                            {
                                day = Convert.ToInt64(Convert.ToDateTime(dr["DAYD"]).ToString("yyyyMMdd")),
                                id_login = Convert.ToInt64(dr["ID_LOGIN"].ToString()),
                                login = dr["LOGIN"].ToString(),
                                connectionNumber = Convert.ToInt64(dr["NB_CO"].ToString()),

                            });

                        }
                    }
                    jsonObj = JsonConvert.SerializeObject(objs);

                    File.WriteAllText(Path.Combine(projectDirectory, Path.Combine("output", "dayTracking.json")), jsonObj);
                }

            }

            return Path.Combine(projectDirectory, Path.Combine("output", "dayTracking.json"));

        }

        public string ExportMediaDataToJsonFile()
        {
            ArrayList objs = new ArrayList();
            string jsonObj = string.Empty;

            using (OracleConnection con = new OracleConnection(connectionString))
            {
                using (OracleCommand cmd = new OracleCommand())
                {
                    con.Open();
                    cmd.Connection = con;

                    cmd.CommandText = @"
                        SELECT DATE_CONNECTION AS DAY_CON, v.ID_VEHICLE, VEHICLE, tv.ID_LOGIN, LOGIN, SUM(CONNECTION_NUMBER) as CONNECTION_NUMBER
                        FROM WEBNAV02.TOP_VEHICLE tv, ADEXPR03.VEHICLE v, MAU01.LOGIN l
                        WHERE TV.ID_VEHICLE  = v.ID_VEHICLE
                        AND tv.id_login = l.id_login
                        AND v.ID_LANGUAGE = 33
                        AND DATE_CONNECTION >= 20161001
                        GROUP BY   DATE_CONNECTION, v.ID_VEHICLE, VEHICLE, tv.ID_LOGIN, LOGIN
                        ORDER BY  DATE_CONNECTION,VEHICLE
                    ";

                    dr = cmd.ExecuteReader();

                    if (dr.HasRows)
                    {
                        while (dr.Read())
                        {
                            objs.Add(new
                            {
                                day = Convert.ToInt64(dr["DAY_CON"].ToString()),
                                media = dr["VEHICLE"].ToString(),
                                id_login = Convert.ToInt64(dr["ID_LOGIN"].ToString()),
                                login = dr["LOGIN"].ToString(),
                                connectionNumber = Convert.ToInt64(dr["CONNECTION_NUMBER"].ToString())
                            });

                        }
                    }
                    jsonObj = JsonConvert.SerializeObject(objs);

                    File.WriteAllText(Path.Combine(projectDirectory, Path.Combine("output", "mediaTracking.json")), jsonObj);
                }

            }

            return Path.Combine(projectDirectory, Path.Combine("output", "mediaTracking.json"));

        }

        public string ExportTypologyDataToJsonFile()
        {
            ArrayList objs = new ArrayList();
            string jsonObj = string.Empty;

            using (OracleConnection con = new OracleConnection(connectionString))
            {
                using (OracleCommand cmd = new OracleCommand())
                {
                    con.Open();
                    cmd.Connection = con;

                    cmd.CommandText = @"
                        SELECT DATE_CONNECTION AS DAY_CON, g.ID_GROUP_CONTACT, GROUP_CONTACT, t.ID_LOGIN, LOGIN, SUM(CONNECTION_NUMBER) as CONNECTION_NUMBER
                        FROM WEBNAV02.TOP_MEDIA_AGENCY t, MAU01.LOGIN l, MAU01.CONTACT c, MAU01.GROUP_CONTACT g
                        WHERE t.ID_LOGIN = l.ID_LOGIN
                        AND l.ID_CONTACT = c.ID_CONTACT
                        AND c.ID_GROUP_CONTACT = g.ID_GROUP_CONTACT
                        AND DATE_CONNECTION >= 20161001
                        GROUP BY   DATE_CONNECTION, g.ID_GROUP_CONTACT, GROUP_CONTACT, t.ID_LOGIN, LOGIN
                        ORDER BY  DATE_CONNECTION, GROUP_CONTACT
                    ";

                    dr = cmd.ExecuteReader();

                    if (dr.HasRows)
                    {
                        while (dr.Read())
                        {
                            objs.Add(new
                            {
                                day = Convert.ToInt64(dr["DAY_CON"].ToString()),
                                typology = dr["GROUP_CONTACT"].ToString(),
                                id_login = Convert.ToInt64(dr["ID_LOGIN"].ToString()),
                                login = dr["LOGIN"].ToString(),
                                connectionNumber = Convert.ToInt64(dr["CONNECTION_NUMBER"].ToString())
                            });

                        }
                    }
                    jsonObj = JsonConvert.SerializeObject(objs);

                    File.WriteAllText(Path.Combine(projectDirectory, Path.Combine("output", "typologyTracking.json")), jsonObj);
                }

            }

            return Path.Combine(projectDirectory, Path.Combine("output", "typologyTracking.json"));

        }

        public string ExportModuleDataToJsonFile()
        {
            ArrayList objs = new ArrayList();
            string jsonObj = string.Empty;

            using (OracleConnection con = new OracleConnection(connectionString))
            {
                using (OracleCommand cmd = new OracleCommand())
                {
                    con.Open();
                    cmd.Connection = con;

                    cmd.CommandText = @"
                        SELECT DATE_CONNECTION AS DAY_CON, M.ID_MODULE, MODULE, TM.ID_LOGIN, LOGIN, SUM(CONNECTION_NUMBER) as CONNECTION_NUMBER
                        FROM WEBNAV02.TOP_MODULE TM, MAU01.MODULE M, MAU01.LOGIN L
                        WHERE TM.ID_MODULE = M.ID_MODULE
                        AND TM.ID_LOGIN = L.ID_LOGIN
                        AND DATE_CONNECTION >= 20161001
                        GROUP BY   DATE_CONNECTION, M.ID_MODULE, MODULE, TM.ID_LOGIN, LOGIN
                        ORDER BY  DATE_CONNECTION, MODULE
                    ";

                    dr = cmd.ExecuteReader();

                    if (dr.HasRows)
                    {
                        while (dr.Read())
                        {
                            objs.Add(new
                            {
                                day = Convert.ToInt64(dr["DAY_CON"].ToString()),
                                module = dr["MODULE"].ToString(),
                                id_login = Convert.ToInt64(dr["ID_LOGIN"].ToString()),
                                login = dr["LOGIN"].ToString(),
                                connectionNumber = Convert.ToInt64(dr["CONNECTION_NUMBER"].ToString())
                            });

                        }
                    }
                    jsonObj = JsonConvert.SerializeObject(objs);

                    File.WriteAllText(Path.Combine(projectDirectory, Path.Combine("output", "moduleTracking.json")), jsonObj);
                }

            }

            return Path.Combine(projectDirectory, Path.Combine("output", "moduleTracking.json"));

        }

        public string ExportUserSessionDataToJsonFile()
        {
            ArrayList objs = new ArrayList();
            string jsonObj = string.Empty;

            using (OracleConnection con = new OracleConnection(connectionString))
            {
                using (OracleCommand cmd = new OracleCommand())
                {
                    con.Open();
                    cmd.Connection = con;

                    cmd.CommandText = @"

SELECT
                                  ID_NAV_SESSION,
                                  c.ID_CONTACT,
                                  c.FIRST_NAME,
                                  c.NAME,
                                  l.ID_LOGIN,
                                  l.LOGIN,
                                  m.ID_MODULE,
                                  m.MODULE,
                                  r.ID_RESULT,
                                  r.RESULT,
                                   c.ID_GROUP_CONTACT,
                                  gc.GROUP_CONTACT,
                                  e.ID_EVENT,
                                  'CONNEXION AU SITE' as EVENT,
                                  VALUE AS ID_VALUE,
                                 VALUE_STRING,
                                  TRUNC(ta.DATE_CREATION, 'DD') AS DAYD,
                                  TRUNC(ta.DATE_CREATION, 'HH24') AS HOURH
FROM WEBNAV02.TRACKING_ARCHIVE ta
                                  LEFT OUTER JOIN MAU01.MODULE m ON
                                    ta.ID_MODULE = m.ID_MODULE
                                  LEFT OUTER JOIN MAU01.RESULT r ON
                                    ta.ID_RESULT = r.ID_RESULT
                                  INNER JOIN MAU01.LOGIN l ON
                                    ta.ID_LOGIN = l.ID_LOGIN
                                  INNER JOIN MAU01.CONTACT c ON
                                    l.ID_CONTACT = c.ID_CONTACT
                                     INNER JOIN MAU01.GROUP_CONTACT gc ON
                                    c.ID_GROUP_CONTACT = gc.ID_GROUP_CONTACT
                                  INNER JOIN WEBNAV02.EVENT e ON
                                    ta.ID_EVENT = e.ID_EVENT
WHERE  ta.DATE_CREATION >= TO_DATE('01/01/2015', 'DD/MM/YYYY')
                                AND e.ID_EVENT  = 1

 UNION ALL

SELECT
                                  ID_NAV_SESSION,
                                  c.ID_CONTACT,
                                  c.FIRST_NAME,
                                  c.NAME,
                                  l.ID_LOGIN,
                                  l.LOGIN,
                                  m.ID_MODULE,
                                  m.MODULE,
                                  r.ID_RESULT,
                                  r.RESULT,
                                   c.ID_GROUP_CONTACT,
                                  gc.GROUP_CONTACT,
                                  e.ID_EVENT,
                                  'MODULE SELECTIONNE' as EVENT,
                                  VALUE AS ID_VALUE,
                                  mo.MODULE as VALUE_STRING,
                                  TRUNC(ta.DATE_CREATION, 'DD') AS DAYD,
                                  TRUNC(ta.DATE_CREATION, 'HH24') AS HOURH
                                FROM WEBNAV02.TRACKING_ARCHIVE ta
                                  LEFT OUTER JOIN MAU01.MODULE m ON
                                    ta.ID_MODULE = m.ID_MODULE
                                  LEFT OUTER JOIN MAU01.RESULT r ON
                                    ta.ID_RESULT = r.ID_RESULT
                                  INNER JOIN MAU01.LOGIN l ON
                                    ta.ID_LOGIN = l.ID_LOGIN
                                  INNER JOIN MAU01.CONTACT c ON
                                    l.ID_CONTACT = c.ID_CONTACT
                                     INNER JOIN MAU01.GROUP_CONTACT gc ON
                                    c.ID_GROUP_CONTACT = gc.ID_GROUP_CONTACT
                                  INNER JOIN WEBNAV02.EVENT e ON
                                    ta.ID_EVENT = e.ID_EVENT
                                  INNER JOIN MAU01.MODULE mo ON
                                    ta.VALUE = mo.ID_MODULE
                                WhERE  ta.DATE_CREATION >= TO_DATE('01/01/2015', 'DD/MM/YYYY')
                                  AND e.ID_EVENT  = 2
 UNION ALL

SELECT
                                  ID_NAV_SESSION,
                                  c.ID_CONTACT,
                                  c.FIRST_NAME,
                                  c.NAME,
                                  l.ID_LOGIN,
                                  l.LOGIN,
                                  m.ID_MODULE,
                                  m.MODULE,
                                  r.ID_RESULT,
                                  r.RESULT,
                                   c.ID_GROUP_CONTACT,
                                  gc.GROUP_CONTACT,
                                  e.ID_EVENT,
                                  'MEDIA' as EVENT,
                                  VALUE AS ID_VALUE,
                                  vh.VEHICLE as VALUE_STRING,
                                  TRUNC(ta.DATE_CREATION, 'DD') AS DAYD,
                                  TRUNC(ta.DATE_CREATION, 'HH24') AS HOURH
FROM WEBNAV02.TRACKING_ARCHIVE ta
                                  LEFT OUTER JOIN MAU01.MODULE m ON
                                    ta.ID_MODULE = m.ID_MODULE
                                  LEFT OUTER JOIN MAU01.RESULT r ON
                                    ta.ID_RESULT = r.ID_RESULT
                                  INNER JOIN MAU01.LOGIN l ON
                                    ta.ID_LOGIN = l.ID_LOGIN
                                  INNER JOIN MAU01.CONTACT c ON
                                    l.ID_CONTACT = c.ID_CONTACT
                                     INNER JOIN MAU01.GROUP_CONTACT gc ON
                                    c.ID_GROUP_CONTACT = gc.ID_GROUP_CONTACT
                                  INNER JOIN WEBNAV02.EVENT e ON
                                    ta.ID_EVENT = e.ID_EVENT
                                  LEFT OUTER JOIN ADEXPR03.VEHICLE vh ON
                                    ta.VALUE = vh.ID_VEHICLE
WHERE  ta.DATE_CREATION >= TO_DATE('01/01/2015', 'DD/MM/YYYY')
                                AND e.ID_EVENT  = 3
                                AND vh.ID_LANGUAGE = 33


UNION ALL


SELECT
                                  ID_NAV_SESSION,
                                  c.ID_CONTACT,
                                  c.FIRST_NAME,
                                  c.NAME,
                                  l.ID_LOGIN,
                                  l.LOGIN,
                                  m.ID_MODULE,
                                  m.MODULE,
                                  r.ID_RESULT,
                                  r.RESULT,
                                   c.ID_GROUP_CONTACT,
                                  gc.GROUP_CONTACT,
                                  e.ID_EVENT,
                                  'GAD' as EVENT,
                                  VALUE AS ID_VALUE,
                                  VALUE_STRING,
                                  TRUNC(ta.DATE_CREATION, 'DD') AS DAYD,
                                  TRUNC(ta.DATE_CREATION, 'HH24') AS HOURH
                                FROM WEBNAV01.TRACKING_ARCHIVE ta
                                  LEFT OUTER JOIN MAU01.MODULE m ON
                                    ta.ID_MODULE = m.ID_MODULE
                                  LEFT OUTER JOIN MAU01.RESULT r ON
                                    ta.ID_RESULT = r.ID_RESULT
                                  INNER JOIN MAU01.LOGIN l ON
                                    ta.ID_LOGIN = l.ID_LOGIN
                                  INNER JOIN MAU01.CONTACT c ON
                                    l.ID_CONTACT = c.ID_CONTACT
                                     INNER JOIN MAU01.GROUP_CONTACT gc ON
                                    c.ID_GROUP_CONTACT = gc.ID_GROUP_CONTACT
                                  INNER JOIN WEBNAV01.EVENT e ON
                                    ta.ID_EVENT = e.ID_EVENT
                                WHERE  ta.DATE_CREATION >= TO_DATE('01/01/2015', 'DD/MM/YYYY')
                                AND e.ID_EVENT  = 4
  UNION ALL

	SELECT
                                  ID_NAV_SESSION,
                                  c.ID_CONTACT,
                                  c.FIRST_NAME,
                                  c.NAME,
                                  l.ID_LOGIN,
                                  l.LOGIN,
                                  m.ID_MODULE,
                                  m.MODULE,
                                  r.ID_RESULT,
                                  r.RESULT,
                                   c.ID_GROUP_CONTACT,
                                  gc.GROUP_CONTACT,
                                  e.ID_EVENT,
                                  'CHOIX ANGENCE MEDIA' AS EVENT,
                                  VALUE AS ID_VALUE,
                                  VALUE_STRING,
                                  TRUNC(ta.DATE_CREATION, 'DD') AS DAYD,
                                  TRUNC(ta.DATE_CREATION, 'HH24') AS HOURH
                                FROM WEBNAV02.TRACKING_ARCHIVE ta
                                  LEFT OUTER JOIN MAU01.MODULE m ON
                                    ta.ID_MODULE = m.ID_MODULE
                                  LEFT OUTER JOIN MAU01.RESULT r ON
                                    ta.ID_RESULT = r.ID_RESULT
                                  INNER JOIN MAU01.LOGIN l ON
                                    ta.ID_LOGIN = l.ID_LOGIN
                                  INNER JOIN MAU01.CONTACT c ON
                                    l.ID_CONTACT = c.ID_CONTACT
                                     INNER JOIN MAU01.GROUP_CONTACT gc ON
                                    c.ID_GROUP_CONTACT = gc.ID_GROUP_CONTACT
                                  INNER JOIN WEBNAV02.EVENT e ON
                                    ta.ID_EVENT = e.ID_EVENT
                                WHERE  ta.DATE_CREATION >= TO_DATE('01/01/2015', 'DD/MM/YYYY')
                                AND e.ID_EVENT  = 5
 UNION ALL

SELECT
                                  ID_NAV_SESSION,
                                  c.ID_CONTACT,
                                  c.FIRST_NAME,
                                  c.NAME,
                                  l.ID_LOGIN,
                                  l.LOGIN,
                                  m.ID_MODULE,
                                  m.MODULE,
                                  r.ID_RESULT,
                                  r.RESULT,
                                   c.ID_GROUP_CONTACT,
                                  gc.GROUP_CONTACT,
                                  e.ID_EVENT,
                                  'TYPE PERIODE SELECTIONNE' as EVENT,
                                  VALUE AS ID_VALUE,
                                  vh.PERIODE as VALUE_STRING,
                                  TRUNC(ta.DATE_CREATION, 'DD') AS DAYD,
                                  TRUNC(ta.DATE_CREATION, 'HH24') AS HOURH
FROM WEBNAV02.TRACKING_ARCHIVE ta
                                  LEFT OUTER JOIN MAU01.MODULE m ON
                                    ta.ID_MODULE = m.ID_MODULE
                                  LEFT OUTER JOIN MAU01.RESULT r ON
                                    ta.ID_RESULT = r.ID_RESULT
                                  INNER JOIN MAU01.LOGIN l ON
                                    ta.ID_LOGIN = l.ID_LOGIN
                                  INNER JOIN MAU01.CONTACT c ON
                                    l.ID_CONTACT = c.ID_CONTACT
                                     INNER JOIN MAU01.GROUP_CONTACT gc ON
                                    c.ID_GROUP_CONTACT = gc.ID_GROUP_CONTACT
                                  INNER JOIN WEBNAV02.EVENT e ON
                                    ta.ID_EVENT = e.ID_EVENT
                                  LEFT OUTER JOIN WEBNAV02.PERIODE vh ON
                                    ta.VALUE = vh.ID_PERIODE
WHERE  ta.DATE_CREATION >= TO_DATE('01/01/2015', 'DD/MM/YYYY')
                                AND e.ID_EVENT  = 6
UNION ALL

SELECT
                                  ID_NAV_SESSION,
                                  c.ID_CONTACT,
                                  c.FIRST_NAME,
                                  c.NAME,
                                  l.ID_LOGIN,
                                  l.LOGIN,
                                  m.ID_MODULE,
                                  m.MODULE,
                                  r.ID_RESULT,
                                  r.RESULT,
                                   c.ID_GROUP_CONTACT,
                                  gc.GROUP_CONTACT,
                                  e.ID_EVENT,
                                  'TYPE UNITE SELECTIONNE' as EVENT,
                                  VALUE AS ID_VALUE,
                                  vh.UNIT as VALUE_STRING,
                                  TRUNC(ta.DATE_CREATION, 'DD') AS DAYD,
                                  TRUNC(ta.DATE_CREATION, 'HH24') AS HOURH
FROM WEBNAV02.TRACKING_ARCHIVE ta
                                  LEFT OUTER JOIN MAU01.MODULE m ON
                                    ta.ID_MODULE = m.ID_MODULE
                                  LEFT OUTER JOIN MAU01.RESULT r ON
                                    ta.ID_RESULT = r.ID_RESULT
                                  INNER JOIN MAU01.LOGIN l ON
                                    ta.ID_LOGIN = l.ID_LOGIN
                                  INNER JOIN MAU01.CONTACT c ON
                                    l.ID_CONTACT = c.ID_CONTACT
                                     INNER JOIN MAU01.GROUP_CONTACT gc ON
                                    c.ID_GROUP_CONTACT = gc.ID_GROUP_CONTACT
                                  INNER JOIN WEBNAV02.EVENT e ON
                                    ta.ID_EVENT = e.ID_EVENT
                                  LEFT OUTER JOIN WEBNAV02.UNIT vh ON
                                    ta.VALUE = vh.ID_UNIT
WHERE  ta.DATE_CREATION >= TO_DATE('01/01/2015', 'DD/MM/YYYY')
                                AND e.ID_EVENT  = 7

 UNION ALL

SELECT
                                  ID_NAV_SESSION,
                                  c.ID_CONTACT,
                                  c.FIRST_NAME,
                                  c.NAME,
                                  l.ID_LOGIN,
                                  l.LOGIN,
                                  m.ID_MODULE,
                                  m.MODULE,
                                  r.ID_RESULT,
                                  r.RESULT,
                                   c.ID_GROUP_CONTACT,
                                  gc.GROUP_CONTACT,
                                  e.ID_EVENT,
                                  'TYPE RESULTAT' as EVENT,
                                  VALUE AS ID_VALUE,
                                   vh.RESULT as VALUE_STRING,
                                  TRUNC(ta.DATE_CREATION, 'DD') AS DAYD,
                                  TRUNC(ta.DATE_CREATION, 'HH24') AS HOURH
FROM WEBNAV02.TRACKING_ARCHIVE ta
                                  LEFT OUTER JOIN MAU01.MODULE m ON
                                    ta.ID_MODULE = m.ID_MODULE
                                  LEFT OUTER JOIN MAU01.RESULT r ON
                                    ta.ID_RESULT = r.ID_RESULT
                                  INNER JOIN MAU01.LOGIN l ON
                                    ta.ID_LOGIN = l.ID_LOGIN
                                  INNER JOIN MAU01.CONTACT c ON
                                    l.ID_CONTACT = c.ID_CONTACT
                                     INNER JOIN MAU01.GROUP_CONTACT gc ON
                                    c.ID_GROUP_CONTACT = gc.ID_GROUP_CONTACT
                                  INNER JOIN WEBNAV02.EVENT e ON
                                    ta.ID_EVENT = e.ID_EVENT
                                  INNER JOIN MAU01.RESULT vh ON
                                    ta.VALUE = vh.ID_RESULT
WHERE  ta.DATE_CREATION >= TO_DATE('01/01/2015', 'DD/MM/YYYY')
                                AND e.ID_EVENT  = 8


 UNION ALL


SELECT
                                  ID_NAV_SESSION,
                                  c.ID_CONTACT,
                                  c.FIRST_NAME,
                                  c.NAME,
                                  l.ID_LOGIN,
                                  l.LOGIN,
                                  m.ID_MODULE,
                                  m.MODULE,
                                  r.ID_RESULT,
                                  r.RESULT,
                                   c.ID_GROUP_CONTACT,
                                  gc.GROUP_CONTACT,
                                  e.ID_EVENT,
                                  'DEMANDE EXPORT' as EVENT,
                                  VALUE AS ID_VALUE,
                                   VALUE_STRING,
                                  TRUNC(ta.DATE_CREATION, 'DD') AS DAYD,
                                  TRUNC(ta.DATE_CREATION, 'HH24') AS HOURH
FROM WEBNAV02.TRACKING_ARCHIVE ta
                                  LEFT OUTER JOIN MAU01.MODULE m ON
                                    ta.ID_MODULE = m.ID_MODULE
                                  LEFT OUTER JOIN MAU01.RESULT r ON
                                    ta.ID_RESULT = r.ID_RESULT
                                  INNER JOIN MAU01.LOGIN l ON
                                    ta.ID_LOGIN = l.ID_LOGIN
                                  INNER JOIN MAU01.CONTACT c ON
                                    l.ID_CONTACT = c.ID_CONTACT
                                     INNER JOIN MAU01.GROUP_CONTACT gc ON
                                    c.ID_GROUP_CONTACT = gc.ID_GROUP_CONTACT
                                  INNER JOIN WEBNAV02.EVENT e ON
                                    ta.ID_EVENT = e.ID_EVENT
                                 -- INNER JOIN MAU01.RESULT vh ON
                                  --  ta.VALUE = vh.ID_RESULT
WHERE  ta.DATE_CREATION >= TO_DATE('01/01/2015', 'DD/MM/YYYY')
                                AND e.ID_EVENT  = 9

 UNION ALL

SELECT
                                  ID_NAV_SESSION,
                                  c.ID_CONTACT,
                                  c.FIRST_NAME,
                                  c.NAME,
                                  l.ID_LOGIN,
                                  l.LOGIN,
                                  m.ID_MODULE,
                                  m.MODULE,
                                  r.ID_RESULT,
                                  r.RESULT,
                                   c.ID_GROUP_CONTACT,
                                  gc.GROUP_CONTACT,
                                  e.ID_EVENT,
                                  'UTILISATION MON ADEXPRESS' as EVENT,
                                  VALUE AS ID_VALUE,
                                 VALUE_STRING,
                                  TRUNC(ta.DATE_CREATION, 'DD') AS DAYD,
                                  TRUNC(ta.DATE_CREATION, 'HH24') AS HOURH
FROM WEBNAV02.TRACKING_ARCHIVE ta
                                  LEFT OUTER JOIN MAU01.MODULE m ON
                                    ta.ID_MODULE = m.ID_MODULE
                                  LEFT OUTER JOIN MAU01.RESULT r ON
                                    ta.ID_RESULT = r.ID_RESULT
                                  INNER JOIN MAU01.LOGIN l ON
                                    ta.ID_LOGIN = l.ID_LOGIN
                                  INNER JOIN MAU01.CONTACT c ON
                                    l.ID_CONTACT = c.ID_CONTACT
                                     INNER JOIN MAU01.GROUP_CONTACT gc ON
                                    c.ID_GROUP_CONTACT = gc.ID_GROUP_CONTACT
                                  INNER JOIN WEBNAV02.EVENT e ON
                                    ta.ID_EVENT = e.ID_EVENT
                                 -- INNER JOIN MAU01.RESULT vh ON
                                  --  ta.VALUE = vh.ID_RESULT
WHERE  ta.DATE_CREATION >= TO_DATE('01/01/2015', 'DD/MM/YYYY')
                                AND e.ID_EVENT  = 10




                    ";

                    dr = cmd.ExecuteReader();

                    if (dr.HasRows)
                    {
                        while (dr.Read())
                        {
                            objs.Add(new
                            {
                                idNavSession = Convert.ToInt64(dr["ID_NAV_SESSION"].ToString()),
                                idContact = Convert.ToInt64(dr["ID_CONTACT"].ToString()),
                                contact = dr["NAME"].ToString() +  " " + dr["FIRST_NAME"].ToString(),
                                idLogin = Convert.ToInt64(dr["ID_LOGIN"].ToString()),
                                login = dr["LOGIN"].ToString(),
                                idModule = String.IsNullOrEmpty(dr["ID_MODULE"].ToString()) ? 0 : Convert.ToInt64(dr["ID_MODULE"].ToString()),
                                module = dr["MODULE"].ToString(),
                                idResult = String.IsNullOrEmpty(dr["ID_RESULT"].ToString()) ? 0 : Convert.ToInt64(dr["ID_RESULT"].ToString()),
                                result = dr["RESULT"].ToString(),
                                idEvent = String.IsNullOrEmpty(dr["ID_EVENT"].ToString()) ? 0 :  Convert.ToInt64(dr["ID_EVENT"].ToString()),
                                _event = dr["EVENT"].ToString(),
                                idValue = String.IsNullOrEmpty(dr["ID_VALUE"].ToString()) ? 0 : Convert.ToInt64(dr["ID_VALUE"].ToString()),
                                valueLabel = dr["VALUE_STRING"].ToString(),
                                day = Convert.ToInt64(Convert.ToDateTime(dr["DAYD"]).ToString("yyyyMMdd")),
                                hour = Convert.ToInt64(Convert.ToDateTime(dr["HOURH"]).ToString("yyyyMMddHH")),
                                idTypology = Convert.ToInt64(dr["ID_GROUP_CONTACT"].ToString()),
                                typology = dr["GROUP_CONTACT"].ToString()
                            }
                            );

                        }
                    }
                   jsonObj = JsonConvert.SerializeObject(objs);

                    File.WriteAllText(Path.Combine(projectDirectory, Path.Combine("output", "userSessions.json")), jsonObj);
                }

            }

            return Path.Combine(projectDirectory, Path.Combine("output", "userSessions.json"));

        }

        private OracleDataReader getData(string query)
        {
            OracleDataReader dr;
            object tempObj = new object();

            using (OracleConnection con = new OracleConnection(connectionString))
            {
                using (OracleCommand cmd = new OracleCommand())
                {
                    con.Open();
                    cmd.Connection = con;
                    cmd.CommandText = query;
                    dr = cmd.ExecuteReader();

                    return dr;
                }

            }
        }

    }
}
