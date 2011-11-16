using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace Com.Dover.Web.Models {
	public class AccountViewModel {
		public int Id { get; set; }
		
		[Required(AllowEmptyStrings = false, ErrorMessage = "O nome da conta é obrigatório.")]
		[DisplayName("Nome da conta:")]
		public string Name { get; set; }
		
		[DisplayName("Logotipo:")]
		public string Logo { get; set; }
		
		[Required(AllowEmptyStrings = false, ErrorMessage = "O nome do subdomínio é obrigatório.")]
		[RegularExpression("^([a-zA-Z0-9_-]+)$", ErrorMessage = "Subdomínio inválido. Por favor, utilize apenas letras e números.")]
		[DisplayName("Subdomínio:")]
		public string Subdomain { get; set; }

		public List<string> Users { get; set; }
	}
}