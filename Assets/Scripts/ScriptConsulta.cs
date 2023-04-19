using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MySql.Data.MySqlClient;
using MySqlConnector;
using UnityEngine.UI;
using System;
using System.Net.Sockets;
using System.Text;
using System.Net;
using UnityEngine.UI;

public class ConsultaMySQL : MonoBehaviour
{
    public Button Consulta1;
    public Button Consulta2;
    public Button Consulta3;

    private MySqlConnection connection;
    private string server = "127.0.0.1";
    private string database = "JUGADOR";
    private string username = "root";
    private string password = "mimara";

    void Start()
    {
        // Conectar a la base de datos
        string connectionString = "Server=" + server + ";" + "Database=" +
            database + ";" + "username=" + username + ";" + "Passwordwd=" + password + ";";
        connection = new MySqlConnection(connectionString);
        connection.Open();

        Consulta1.onClick.AddListener(MenosPartidasGanadas);
        Consulta2.onClick.AddListener(NombreJugadorDuracionMayor3);
        Consulta3.onClick.AddListener(JugadoresJugadoMismoDiaPere);

    }

    // Realizar consulta 1
    public void MenosPartidasGanadas()
    {
        string query = "SELECT Lista_Partidas.TIEMPOMEDIO, Lista_Partidas.PARTIDASTOTALES " +
            "FROM Lista_Partidas, JUGADOR, PARTIDA " +
            "WHERE Lista_Partidas.idJ = JUGADOR.ID " +
            "AND Lista_Partidas.idP = PARTIDA.ID " +
            "AND Lista_Partidas.MVP = 'Pere'";
        MySqlCommand cmd = new MySqlCommand(query, connection);
        MySqlDataReader dataReader = cmd.ExecuteReader();
        while (dataReader.Read())
        {
            float tiempoMedio = dataReader.GetFloat(0);
            int partidasTotales = dataReader.GetInt32(1);
            Debug.Log("Tiempo medio: " + tiempoMedio + " Partidas totales: " + partidasTotales);
        }
        dataReader.Close();
    }

    // Realizar consulta 2
    public void NombreJugadorDuracionMayor3()
    {
        string query = "SELECT JUGADOR.NOMBRE " +
            "FROM JUGADOR, PARTIDA " +
            "WHERE PARTIDA.DURACION > 3 AND JUGADOR.ID = PARTIDA.GANADOR";
      //  MySqlCommand cmd = new MySqlCommand(query, connection);
      //  MySqlDataReader dataReader = cmd.ExecuteReader();
        while (dataReader.Read())
        {
            string nombreJugador = dataReader.GetString(0);
            Debug.Log("Nombre del jugador: " + nombreJugador);
        }
        dataReader.Close();
    }

    // Realizar consulta 3
    public void JugadoresJugadoMismoDiaPere()
    {
        string query = "SELECT JUGADOR.NOMBRE " +
            "FROM JUGADOR, PARTIDA, Lista_Partidas " +
            "WHERE JUGADOR.ID = Lista_Partidas.idJ " +
            "AND PARTIDA.ID = Lista_Partidas.idP " +
            "AND PARTIDA.FECHA = (SELECT FECHA FROM PARTIDA, JUGADOR, Lista_Partidas " +
            "WHERE JUGADOR.NOMBRE = 'Pere' AND JUGADOR.ID = Lista_Partidas.MVP " +
            "AND PARTIDA.ID = Lista_Partidas.idP)";
        MySqlCommand cmd = new MySqlCommand(query, connection);
        MySqlDataReader dataReader = cmd.ExecuteReader();
        while (dataReader.Read())
        {
            string nombreJugador = dataReader.GetString(0);
            Debug.Log("Nombre del jugador: " + nombreJugador);
        }
        dataReader.Close();
    }
}