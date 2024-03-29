﻿using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using MyVet.Common.Models;
using Newtonsoft.Json;

namespace MyVet.Common.Services
{
    public class ApiService : IApiService
    {
        //para obtener el token le enviamos un request que la combinacion de email y contraseña 
        public async Task<Response> GetTokenAsync(
            string urlBase,
            string servicePrefix,
            string controller,
            TokenRequest request)
        {
            try
            {
                //el token lo serializamos para convertirlo a un string
                var requestString = JsonConvert.SerializeObject(request);
                
                //luego lo codificamos en un UTF8
                var content = new StringContent(requestString, Encoding.UTF8, "application/json");

                //luego creamos un cliente http HttpClient le pasamos de direccion base
                //la urlBase es https://myvetjose.azurewebsites.net/
                var client = new HttpClient
                {
                    BaseAddress = new Uri(urlBase)
                };

                //luego concatenamos el prefijo y el controlador Account/CreateToken
                var url = $"{servicePrefix}{controller}";
                var response = await client.PostAsync(url, content);

                //obtenemos el resultado
                var result = await response.Content.ReadAsStringAsync();

                //si la respuesta es diferente a 200 es porque fallo y decimos porque falló
                if (!response.IsSuccessStatusCode)
                {
                    return new Response
                    {
                        IsSuccess = false,
                        Message = result,
                    };
                }

                //desserializar es coger un string y volverl clase objeto
                //serializar es coger un objeto y volverlo string
                var token = JsonConvert.DeserializeObject<TokenResponse>(result);
                return new Response
                {
                    IsSuccess = true,
                    Result = token
                };
            }
            catch (Exception ex)
            {
                return new Response
                {
                    IsSuccess = false,
                    Message = ex.Message
                };
            }
        }

        public async Task<Response> GetOwnerByEmailAsync(
            string urlBase,
            string servicePrefix,
            string controller,
            string tokenType,
            string accessToken,
            string email)
        {
            try
            {
                var request = new EmailRequest { Email = email };
                var requestString = JsonConvert.SerializeObject(request);
                var content = new StringContent(requestString, Encoding.UTF8, "application/json");
                var client = new HttpClient
                {
                    BaseAddress = new Uri(urlBase)
                };

                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(tokenType, accessToken);
                var url = $"{servicePrefix}{controller}";
                var response = await client.PostAsync(url, content);
                var result = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    return new Response
                    {
                        IsSuccess = false,
                        Message = result,
                    };
                }

                var owner = JsonConvert.DeserializeObject<OwnerResponse>(result);
                return new Response
                {
                    IsSuccess = true,
                    Result = owner
                };
            }
            catch (Exception ex)
            {
                return new Response
                {
                    IsSuccess = false,
                    Message = ex.Message
                };
            }
        }
    }
}
