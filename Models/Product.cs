using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace INATEL_T141_DM106_Final_Homework.Models
{
    public class Product
    {
        //Identificação única do produto; 
        public int Id { get; set; }

        //Nome do produto(com validação de preenchimento obrigatório);
        [Required(ErrorMessage = "The name field is required")]
        public string Name { get; set; }

        //Descrição;
        public string Description { get; set; }

        //Cor; 
        public string Color { get; set; }

        //Modelo em formato string (com validação de preenchimento obrigatório);
        [Required(ErrorMessage = "The model field is required")]
        //[Index(IsUnique = true)]
        public string Model { get; set; }

        //Código em formato string (com validação de preenchimento obrigatório);
        //[Index(IsUnique = true)]
        [Required(ErrorMessage = "The model field is required")]
        public string Code { get; set; }

        //Preço; 
        public double Price { get; set; }

        //Peso; 
        public double Weight { get; set; }

        //Altura; 
        public double Height { get; set; }

        //Largura; 
        public double Width { get; set; }

        //Comprimento;
        public double Length { get; set; }

        //Diâmetro;
        public double Diameter { get; set; }

        //URL da imagem do produto em formato string. 
        public string URL { get; set; }

    }
}