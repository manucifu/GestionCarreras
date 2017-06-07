using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProyectoCarreras
{
    class Corredor
    {
        private String id , nombre, apellidos, dni, fecha_nac;
        public Corredor(String id, String nombre, String apellidos, String dni, String dorsal, String fecha_nac)
        {
            this.id = id;
            this.nombre = nombre;
            this.apellidos = apellidos;
            this.dni = dni;
            this.fecha_nac = fecha_nac;
        }

        public Corredor()
        {}

        public String getId()
        {
            return id;
        }
        public void setId(String id)
        {
            this.id = id;
        }

        public String getNombre()
        {
            return nombre;
        }

        public void setNombre(String nombre)
        {
            this.nombre = nombre;
        }

        public String getApellidos()
        {
            return apellidos;
        }

        public void setApellidos(String apellidos)
        {
            this.apellidos = apellidos;
        }

        public String getDni()
        {
            return dni;
        }

        public void setDni(String dni)
        {
            this.dni = dni;
        }
        public String getFecha_nac()
        {
            return fecha_nac;
        }

        public void setFecha_nac(String fecha_nac)
        {
            this.fecha_nac = fecha_nac;
        }

        public override string ToString()
        {
            return this.nombre + this.apellidos + this.apellidos ;
        }
    }
}
