/*using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Data.SqlClient;

public class ScriptRegister : MonoBehaviour
{
    public InputField NameInput;
    public InputField PasswordInput;
    public InputField ConfirmPasswordInput;
    public InputField IdInput;
    public Button RegisterButton;

    private string connectionString;
    private MySqlConnection conn;

    void start()
    {
        private string server = "127.0.0.1";
        private string database = "JUGADOR";
        private string nombre = "root";
        private string password = "mimara";

       string connectionString = "Server=" + server + ";Database=" + database + ";Nombre=" + nombre + ";Password=" + password + ";";
       conn = new MySqlConnection(connectionString);

       RegisterButton.onClick.AddListener(() => RegistrarUsuario());

    }
    void RegistrarUsuario()
    {
         string Name = NameInput.text;
         string Password = PasswordInput.text;
         string PasswordConfirm = ConfirmPasswordInput.text;
         string ID = IdInput.text;

    if (Password == PasswordConfirm)//añadir mensaje para el caso de que no coincidan
    {

        string query = "INSERT INTO JUGADOR VALUES('" + ID + "'," + Name + ",'" + Password + "')";
        MySqlCommand cmd = new MySqlCommand(query, conn);
        try
        {
            conn.Open();
            cmd.ExecuteNonQuery();
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
    }
}
*/