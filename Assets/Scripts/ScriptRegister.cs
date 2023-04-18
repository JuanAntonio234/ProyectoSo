/*using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using MySql.Data.MySqlClient;
using MySqlConnector;

public class ScriptRegister : MonoBehaviour
{
    public InputField NameInput;
    public InputField PasswordInput;
    public InputField ConfirmPasswordInput;
    public InputField IdInput;
    public Button RegisterButton;

    public string ConnectionString;
    private MySqlConnection conn;

    private string server = "127.0.0.1";
    private string database = "JUGADOR";
    private string nombre = "root";
    private string password = "mimara";


    void start()
    {

        ConnectionString = "Server=" + server + ";Database=" + database + ";Nombre=" + nombre + ";Password=" + password + ";";
        conn = new MySqlConnection(ConnectionString);

        RegisterButton.onClick.AddListener(() => RegistrarUsuario());

    }

    void RegistrarUsuario()
    {
        ///////////////////////////////////de momento da igual si las contraseñas son diferentes funcion no implementada todavia//////////////////////////////////////
        string Name = NameInput.text;
        string Password = PasswordInput.text;
        string PasswordConfirm = ConfirmPasswordInput.text;
        string ID = IdInput.text;

        string query = "INSERT INTO JUGADOR VALUES('" + ID + "'," + Name + ",'" + Password + "')";
        MySqlCommand cmd = new MySqlCommand(query, conn);

        try
        {
            conn.Open();
            cmd.ExecuteNonQuery();
            Debug.Log("usuario bien registrado");
        }
        catch (MySqlException ex)
        {
            Debug.Log("Error al registrar en la base de datos: " + ex.Message);
        }
        finally
        {
            conn.Close();
        }
    }
}*/


//introducir que una vez se haya registrado vaya al menu del juego