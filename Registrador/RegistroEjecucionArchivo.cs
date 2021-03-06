﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Registrador
{
    public class RegistroEjecucionArchivo : IRegistroEjecucion
    {
        string NombreArchivo;
        List<string> ListaErrores;
        List<string> ListaAdvertencias;

        public RegistroEjecucionArchivo(string nombreArchivo)
        {
            NombreArchivo = nombreArchivo;
            ListaErrores = new List<string>();
            ListaAdvertencias = new List<string>();
        }

        public void Registrar(string evento)
        {
            if (NombreArchivo != String.Empty)
            {
                //string folder = Environment.GetFolderPath(System.Environment.SpecialFolder.LocalApplicationData) + "\\" + "GeneradorASNWin";
                string ApplicationName = AppDomain.CurrentDomain.FriendlyName.Split('.')[0];
                string folder = Environment.GetFolderPath(System.Environment.SpecialFolder.LocalApplicationData) + "\\" + ApplicationName;
                if (!Directory.Exists(folder))
                {
                    Directory.CreateDirectory(folder);
                }
                string archivo = folder + "\\" + NombreArchivo + ".txt";
                if (!File.Exists(archivo))
                {
                    //Si archivo no existe, se crea con mensaje inicial de registro:
                    using (StreamWriter archivoRegistroNuevo = File.CreateText(archivo))
                    {
                        archivoRegistroNuevo.WriteLine(evento);
                    }
                }
                else
                {
                    using (StreamWriter archivoRegistro = File.AppendText(archivo))
                    {
                        archivoRegistro.WriteLine(evento);
                    }
                }
            }
        }

        public void RegistrarError(string error)
        {
            string textoError = "ERROR: " + error;
            Registrar(textoError);
            ListaErrores.Add(textoError);
        }

        public List<string> Errores()
        {
            return ListaErrores;
        }

        public void RegistrarAdvertencia(string advertencia)
        {
            string textoAdvertencia = "ADVERTENCIA: " + advertencia;
            Registrar(textoAdvertencia);
            ListaAdvertencias.Add(textoAdvertencia);
        }

        public List<string> Advertencias()
        {
            return ListaAdvertencias;
        }
    }
}
