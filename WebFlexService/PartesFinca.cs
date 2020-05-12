using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


namespace WebFlexService
{
    

    public class PartesFinca
    {
        public List<CentroDeCostos> centroDeCostos { get; set; }
        public List<object> cuadrillas { get; set; }
        public string descripcion { get; set; }
        public string fechaActual { get; set; }
        public string fechaDeCargar { get; set; }
        public int id { get; set; }
        public int idUnico { get; set; }
        public bool jsonEnviado { get; set; }
        public List<Maquinaria> maquinarias { get; set; }
        public List<MateriasPrima> materiasPrimas { get; set; }
        public List<Operario> operarios { get; set; }
        public List<Parcela> parcelas { get; set; }
        public List<Tratamiento> tratamiento { get; set; }
        public string Observaciones { get; set; }
    }
    public class CentroDeCostos
    {
        public int CtrCst { get; set; }
        public string CtrDsc { get; set; }
        public bool selected { get; set; }
    }

    public class Maquinaria
    {
        public double MqAmorPdte { get; set; }
        public double MqAnyAmor { get; set; }
        public int MqAnyComp { get; set; }
        public int MqCod { get; set; }
        public double MqCostHora { get; set; }
        public double MqImpAmor { get; set; }
        public double MqValAmor { get; set; }
        public double horas { get; set; }
        public bool selected { get; set; }
    }

    public class MateriasPrima
    {
        public int CalifNro { get; set; }
        public int Cod_medida { get; set; }
        public int ElabColor { get; set; }
        public double ElabMin { get; set; }
        public string ElabNro { get; set; }
        public int LOTE { get; set; }
        public double PrdCapEnv { get; set; }
        public int PrdCapMed { get; set; }
        public int STipoNro { get; set; }
        public int SpecNro { get; set; }
        public int VinoCod { get; set; }
        public double factorLitr { get; set; }
        public bool selected { get; set; }
        public double unidades { get; set; }
        public string LoteRef { get; set; }
        public double LoteCant { get; set; }
        public string Deposito { get; set; }
    }

    public class Operario
    {
        public int PersCdrNro { get; set; }
        public int PersCod { get; set; }
        public double horas { get; set; }
        public double cantidad { get; set; }
        public bool selected { get; set; }
        public bool visible { get; set; }
    }

    public class Parcela
    {
        public string CuadDes { get; set; }
        public int CuadNro { get; set; }
        public int CuadPlnts { get; set; }
        public double CuadSup { get; set; }
        public int ObjCod { get; set; }
        public string VitSedeTim { get; set; }
        public int VitiNro { get; set; }
        public int hileras { get; set; }
        public bool selected { get; set; }
    }

    public class Tratamiento
    {
        public int MnpCd { get; set; }
        public string OperCod { get; set; }
        public double OperCosto { get; set; }
        public int OperDigito { get; set; }
        public int OperMedCod { get; set; }
        public int OperVtoDef { get; set; }
        public double dosis { get; set; }
        public bool selected { get; set; }
    }


    /*
    public class PartesFinca
    {
        public int  id { get; set; }
        public string id_unicotablet { get; set; }
        public DateTime fecha_parte { get; set; }
        public DateTime fecha_carga { get; set; }
        public string descripcion { get; set; }
        public DateTime InsertDate { get; set; }
        public DateTime ProcessDate { get; set; }
        public int Estado { get; set; }
        public string Observaciones { get; set; }
        public bool Autorizado { get; set; }
        public DateTime LastUpdate { get; set; }
        public List<Tratamiento> Tratamientos = new List<Tratamiento> {};
        public Tratamiento tratamiento { get; set; }
        public List<Parcela> Parcelas = new List<Parcela> { };
        public List<Operario> Operarios = new List<Operario> { };
        public List<Material> Materiales = new List<Material> { };
        public List<Maquina> Maquinas = new List<Maquina> { };
        public List<Existencia> Existencias = new List<Existencia> { };
        public List<CentroCosto> CentrosdeCostos = new List<CentroCosto> { };        
        public List<object> cuadrillas { get; set; }

    }
    public class Tratamiento
    {
        public int id { get; set; }
        public int id_parte { get; set; }
        public string OperCod { get; set; }

    }
    public class Parcela
    {
        public int id { get; set; }
        public int id_parte { get; set; }
        public string CuadDes { get; set; }
        public int VitiNro { get; set; }
        public string VitSedeTim { get; set; }
        public float Hileras { get; set; }
    }
    public class Operario
    {
        public int id { get; set; }
        public int id_parte { get; set; }
        public float horas { get; set; }
        public int PersCod { get; set; }	
	
    }
    public class Material
    {
        public int id { get; set; }	
        public int id_parte { get; set; }	
        public string ElabNro { get; set; }	
        public float Unidades { get; set; }		
	
    }
    public class Maquina
    {
        public int id { get; set; }		
        public int id_parte { get; set; }		
        public float Horas { get; set; }		
        public int mqcod { get; set; }			
	
    }
	public class Existencia
    {
        public int id { get; set; }			
        public int id_parte { get; set; }			
	    public string PileCod { get; set; }			
	    public string deposito { get; set; }			
	    public string producto { get; set; }			
	    public string lote { get; set; }			
        public string loteref { get; set; }
        public float LoteCant { get; set; }			
    }
    public class CentroCosto
    {
        public int id { get; set; }			
        public int id_parte { get; set; }			
        public string CtrDsc { get; set; }			
    }
    */
}