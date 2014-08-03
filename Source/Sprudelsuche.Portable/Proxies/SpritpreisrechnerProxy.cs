using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Sprudelsuche.Portable.Model;
using Sprudelsuche.Portable.Proxies.Model;

namespace Sprudelsuche.Portable.Proxies
{
    public class SpritpreisrechnerProxy : IGasPriceInfoProxy
    {
        private const string URL = "http://www.spritpreisrechner.at/ts/GasStationServlet";

        public async Task<GasQueryDownloadResult> DownloadAsync(GasQuery parameter)
        {
            // Prepare POST data
            string postData = Uri.EscapeUriString(parameter.ToPostData());
            var ascii = new UTF8Encoding();
            byte[] bData = ascii.GetBytes(postData);
            var content = new ByteArrayContent(bData);
            content.Headers.ContentType = new MediaTypeHeaderValue("application/x-www-form-urlencoded");

            List<RootObject> result = null;

            try
            {
                using (var client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("*/*"));
                    client.DefaultRequestHeaders.Referrer = new Uri("http://spritpreisrechner.at/ts/map.jsp");

                    HttpResponseMessage response = await client.PostAsync(URL, content);
                    string json = await response.Content.ReadAsStringAsync();

                    result = JsonConvert.DeserializeObject<List<RootObject>>(json);
                }
            }
            catch (Exception ex)
            {
                return new GasQueryDownloadResult()
                {
                    Succeeded = false,
                    ErrorMessage = ex.ToString(),
                    Result = new GasQueryResult(parameter)
                };
            }

            // Errors are reported in a rather weird way so let's be extra careful in retrieving those
            if (result.Count == 1)
            {
                var firstItem = result.First();

                if (firstItem != null)
                {
                    var errorItems = firstItem.errorItems;

                    if (errorItems != null && errorItems.Count > 0 && errorItems.First() != null)
                    {
                        return new GasQueryDownloadResult()
                                   {
                                       Succeeded = false,
                                       ErrorMessage = errorItems.First().msgText,
                                       Result = new GasQueryResult(parameter)
                                   };
                    }
                }
            }
            
            var foundTanken = new List<GasStationResult>();

            foreach (var ro in result)
            {
                if (null == ro) continue;

                if (!String.IsNullOrWhiteSpace(ro.gasStationName))
                {
                    var tanke = new GasStationResult()
                                    {
                                        City = ro.city,
                                        PostalCode = ro.postalCode,
                                        Street = ro.address,
                                        Name = ro.gasStationName,
                                        Longitude = MappingHelpers.ConvertDouble(ro.longitude),
                                        Latitude = MappingHelpers.ConvertDouble(ro.latitude),
                                    };

                    if ((null != ro.spritPrice) && ro.spritPrice.Count > 0)
                    {
                        var preisInfo = ro.spritPrice.First();

                        if ((null != preisInfo) && !String.IsNullOrWhiteSpace(preisInfo.amount))
                        {
                            tanke.Price = MappingHelpers.ConvertDouble(preisInfo.amount);
                            foundTanken.Add(tanke);
                        }
                    }
                }
            }

            var orderedTanken = foundTanken.OrderBy(t => t.Price).ToList();
            int tankeOrderNumber = 1;
            foreach (var tanke in orderedTanken)
            {
                tanke.UniqueId = tankeOrderNumber.ToString();
                tankeOrderNumber++;
            }

            return new GasQueryDownloadResult()
                       {
                           Succeeded = true,
                           Result = new GasQueryResult(parameter, orderedTanken, DateTime.Now)
                       };
        }
    }
}
