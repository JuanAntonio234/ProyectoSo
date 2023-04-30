using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using MySql.Data.MySqlClient;

public class ScriptIniciarSesion : MonoBehaviour
{
    public InputField nameInput;
    public InputField PasswordInput;

    private string connectionString;
    private MySqlConnection dbConnection;
    private string server = "localhost";
    private string database = "JUGADOR";
    private string nombre = "root";
    private string password = "mimara";

    public void Login()
    {
        string nombre = nameInput.text;
        string password = PasswordInput.text;

        if (VerificarUsuario(nombre, password))
        {
            Debug.Log("Inicio de sesión exitoso");
        }
        else
        {
            Debug.Log("Nombre de usuario o contraseña incorrectos");
        }
    }
    private bool VerificarUsuario(string username, string password1)
    {
        // Establecer la conexión a la base de datos
        connectionString = "Server=" + server + ";Database=" + database + ";Uid=" + nombre + ";Pwd=" + password + ";";
        dbConnection = new MySqlConnection(connectionString);
        dbConnection.Open();

        MySqlCommand dbCommand = dbConnection.CreateCommand();
        dbCommand.CommandText = "SELECT COUNT(*) FROM Users WHERE username = '" + username + "' AND password = '" + password1 + "'";
        int count = (int)dbCommand.ExecuteScalar();

        // Cerrar la conexión a la base de datos
        dbConnection.Close();

        if (count > 0)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}


//añadir en el servidor, hacer que nos devuelvva un valor y eso indicara si el usuario es correcto o no
// por consecuencia modificar tambien el codigo del cliente para implementarlo
