using UnityEngine;
using UnityEngine.UI;
using System;
using System.Data;
using MySql.Data.MySqlClient;
using System.Collections;
using System.Collections.Generic;

public class SaveDataRegister : MonoBehaviour
{

    public InputField NameInput;
    public InputField PasswordInput;
    public InputField ConfirmPasswordInput;
    public InputField IdInput;
    public Button RegisterButton;

    private MySqlConnection conn;
    private string server= "127.0.0.1";
    private string database="JUGADOR";
    private string nombre="root";
    private string password="mimara";
    
    void Start()
    {
        RegisterButton.onClick.AddListener(guardarDatosEnBBDD);
    }
     void guardarDatosEnBBDD()
    {
        string Name = NameInput.text;
        string Password = PasswordInput.text;
        string PasswordConfirm = ConfirmPasswordInput.text;
        string ID = IdInput.text;

        string connectionString = "Server=" + server + ";Database=" + database + ";Nombre=" + nombre + ";Password=" + password + ";";
        conn = new MySqlConnection(connectionString);
        try
        {
            conn.Open();


            if (Password == PasswordConfirm)
            {
                string query = "INSERT INTO JUGADOR VALUES('" + ID + "'," + Name + ",'" + Password + "')";
                MySqlCommand cmd = new MySqlCommand(query, conn);
                cmd.ExecuteNonQuery();

                Debug.Log("Fila introducida correctamente.");
            }
            else
            {
                Debug.Log("Las contraseñas no coinciden");
            }
        }
        catch (MySqlException ex)
        {
            Debug.Log("Error al conectar a la base de datos: " + ex.Message);
        }
        conn.Close();
    }
}
