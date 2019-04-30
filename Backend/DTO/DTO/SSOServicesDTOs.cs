﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTO.DTO
{
    public class SSOServicesDTOs
    {
        public class DeleteUserFromSSO_DTO
        {
            [Required]
            public string AppId { get; set; }
            [Required]
            public string Email { get; set; }
            [Required]
            public string Signature { get; set; }
            [Required]
            public string SsoUserId { get; set; }
            [Required]
            public long Timestamp { get; set; }

            public string PreSignatureString()
            {
                string acc = "";
                acc += "ssoUserId=" + SsoUserId + ";";
                acc += "email=" + Email + ";";
                acc += "timestamp=" + Timestamp + ";";
                return acc;
            }
        }
    }
}