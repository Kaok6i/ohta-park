//------------------------------------------------------------------------------
// <auto-generated>
//     Этот код создан по шаблону.
//
//     Изменения, вносимые в этот файл вручную, могут привести к непредвиденной работе приложения.
//     Изменения, вносимые в этот файл вручную, будут перезаписаны при повторном создании кода.
// </auto-generated>
//------------------------------------------------------------------------------

namespace OhtaApp
{
    using System;
    using System.Collections.Generic;
    
    public partial class Employees_Users
    {
        public int ID { get; set; }
        public int ID_Employee { get; set; }
        public string Login { get; set; }
        public string Password { get; set; }
        public System.DateTime Last_Enter { get; set; }
        public bool Enter_Type { get; set; }

        public string NameEnterType
        {
            get
            {
                if (Enter_Type != null)
                {
                    if ((bool)Enter_Type)
                    {
                        return "Успешно";
                    }
                    else
                    {
                        return "Неудачно";
                    }
                }
                else
                {
                    return null;
                }
            }
        }
        public string Last_Login_RU
        {
            get
            {
                if (Last_Enter != null)
                {
                    return string.Format("{0:G}", Last_Enter);
                }
                else
                {
                    return string.Empty;
                }
            }
        }

        public virtual Employees Employees { get; set; }
    }
}
