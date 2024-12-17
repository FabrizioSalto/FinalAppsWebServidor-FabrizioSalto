using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using PayPal;
using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace ClientInterfaz
{
    static class Program
    {
        private static readonly HttpClient client = new HttpClient();

        static async Task Main(string[] args)
        {
            static string CentrarTexto(string texto)
            {
                int espacio = (Console.WindowWidth - texto.Length) / 2;
                return new string(' ', espacio) + texto;
            }
            int opcion;
            try
            {
                do
                {
                    Console.Clear();

                    // Centrado del encabezado
                    string titulo = "Bienvenido al Sistema de";
                    string subtitulo = "Gestión de Clientes";
                    string separador = "------------------------------------------------------------------------------------------------------------------------";

                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine(CentrarTexto(titulo));
                    Console.WriteLine(CentrarTexto(subtitulo));

                    // Opciones con color distinto
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Console.WriteLine(CentrarTexto("1) Crear un cliente"));
                    Console.WriteLine(CentrarTexto("2) Leer un cliente"));
                    Console.WriteLine(CentrarTexto("3) Actualizar un cliente"));
                    Console.WriteLine(CentrarTexto("4) Eliminar un cliente"));
                    Console.WriteLine(CentrarTexto("5) Listado de clientes"));
                    Console.WriteLine(CentrarTexto("0) Salir"));

                    // Restablecer el color después de las opciones
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.Write(CentrarTexto("\nIngrese una opción: "));
                    opcion = Convert.ToInt32(Console.ReadLine());
                    switch (opcion)
                    {
                        case 1: // Crear
                            Console.WriteLine(CentrarTexto("Ingresar Nombre:"));
                            string? nombre = Console.ReadLine();
                            Console.WriteLine(CentrarTexto("Ingresar Email:"));
                            string? email = Console.ReadLine();
                            Console.WriteLine(CentrarTexto("Ingresar Edad:"));
                            int edad = Convert.ToInt32(Console.ReadLine());
                            Console.WriteLine(CentrarTexto("Ingresar Direccion:"));
                            string? calle = Console.ReadLine();
                            Client client = new Client
                            {
                                Name = nombre,
                                Age = edad,
                                Email = email,
                                Address = calle,
                            };
                            Console.WriteLine(nombre + "," + email + "," + edad + "," + calle + ",");
                            await CreateClient(client);
                            Console.WriteLine(CentrarTexto("Presione cualquier tecla para continuar..."));
                            Console.ReadKey();

                            break;
                        case 2: // Leer
                            Console.WriteLine(CentrarTexto("Ingresar Codigo:"));
                            int codigo = Convert.ToInt32(Console.ReadLine());
                            await ReadClient(codigo);
                            Console.WriteLine(separador);
                            Console.WriteLine(CentrarTexto("Presione cualquier tecla para continuar..."));
                            Console.ReadKey();
                            break;
                        case 3: // Actualizar
                            Console.WriteLine(CentrarTexto("Ingresar Codigo:"));
                            codigo = Convert.ToInt32(Console.ReadLine());

                            await ReadClient(codigo); 
                            Console.WriteLine("Esta seguro que eligio el cliente correcto? (s/n)");
                            if (Console.ReadLine() == "s")
                            {
                                Console.WriteLine(CentrarTexto("Ingresar Nombre:"));
                                nombre = Console.ReadLine();
                                Console.WriteLine(CentrarTexto("Ingresar Email:"));
                                email = Console.ReadLine();
                                Console.WriteLine(CentrarTexto("Ingresar Edad:"));
                                edad = Convert.ToInt32(Console.ReadLine());
                                Console.WriteLine(CentrarTexto("Ingresar Direccion:"));
                                calle = Console.ReadLine();
                                client = new Client
                                {
                                    Name = nombre,
                                    Age = edad,
                                    Email = email,
                                    Address = calle,
                                };
                                await UpdateClient(codigo, client);
                            }
                            else
{
                                Console.WriteLine(CentrarTexto("Presione cualquier tecla para continuar..."));
                                Console.ReadKey();
                            }
                            Console.WriteLine(separador);
                            Console.WriteLine(CentrarTexto("Presione cualquier tecla para continuar..."));
                            
                            Console.ReadKey();
                            break;
                        case 4: // Eliminar
                            Console.WriteLine(CentrarTexto("Ingresar Codigo:"));
                            codigo = Convert.ToInt32(Console.ReadLine());
                            await ReadClient(codigo);
                            Console.WriteLine("Esta seguro que eligio el cliente correcto? (s/n)");
                            if (Console.ReadLine() == "s")
                            {
                                await DeleteClient(codigo);
                                Console.WriteLine(separador);
                                Console.WriteLine(CentrarTexto("Presione cualquier tecla para continuar..."));
                                Console.ReadKey();
                            }
                            else
                            {break;
                            }
                            break;
                        case 5: // Listar
                            await ReadAllClients();
                            Console.WriteLine(separador);
                            Console.WriteLine(CentrarTexto("Presione cualquier tecla para continuar..."));
                            Console.ReadKey();
                            break;
                        default:
                            Console.WriteLine(CentrarTexto("Item no contemplado"));
                            break;
                    }
                } while (opcion != 0);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }


        private static async Task CreateClient(Client clientData)
        {
            // Serializa el objeto Client a JSON
            string jsonClient = System.Text.Json.JsonSerializer.Serialize(clientData);
            var content = new StringContent(jsonClient, Encoding.UTF8, "application/json");

            // Realiza la solicitud POST usando la instancia de HttpClient
            HttpResponseMessage response = await client.PostAsync("https://localhost:7013/Clients/CreateClient", content);

            if (response.IsSuccessStatusCode)
            {
                Console.WriteLine("Cliente creado exitosamente.");
            }
            else
            {
                string errorContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Error al crear el Cliente. Código de estado: {response.StatusCode}, Detalles: {errorContent}");
            }
        }


        private static async Task ReadClient(int id)
        {
            string url = $"https://localhost:7013/Clients/ReadClient/{id}";
            var response = await client.GetAsync(url);
            if (response.IsSuccessStatusCode)
            {
                var client = await response.Content.ReadFromJsonAsync<Client>();
                Console.WriteLine($"Name: {client.Name}, Age: {client.Age}, Email: {client.Email}, Address: {client.Address}");
            }
            else
            {
                Console.WriteLine("Error al leer el cliente.");
            }
        }

        private static async Task ReadAllClients()
        {
            string url = "https://localhost:7013/Clients/ReadAllClients";

            var response = await client.GetAsync(url);
            if (response.IsSuccessStatusCode)
            {
                var clients = await response.Content.ReadFromJsonAsync<Client[]>();
                Console.WriteLine("All Clients:");
                foreach (var client in clients)
                {
                    Console.WriteLine($"- {client.Name}, Age: {client.Age}, Email: {client.Email}, Address: {client.Address}");
                }
            }
            else
            {
                Console.WriteLine("Error al obtener los clientes.");
            }
        }

        private static async Task<bool> UpdateClient(int id, Client updatedClient)
        {
            string jsonClient = JsonConvert.SerializeObject(updatedClient);
            var content = new StringContent(jsonClient, Encoding.UTF8, "application/json");

            HttpResponseMessage response = await client.PutAsync($"https://localhost:7013/Clients/UpdateClient/{id}", content);

            if (response.IsSuccessStatusCode)
            {
                Console.WriteLine("Cliente actualizado exitosamente.");
                return true;
            }
            else
            {
                string errorContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Error al actualizar el cliente: {errorContent}");
                return false;
            }
        }

        private static async Task DeleteClient(int id)
        {
            string url = $"https://localhost:7013/Clients/DeleteClient/{id}";
            var response = await client.DeleteAsync(url);
            if (response.IsSuccessStatusCode)
            {
                Console.WriteLine("Client borrado correctamente.");
            }
            else
            {
                Console.WriteLine("Error al eliminar el Cliente.");
            }
        }

       
        
    }
}


public class Client
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Email { get; set; }
        public int Age { get; set; }
        public string? Address { get; set; } 


    }



