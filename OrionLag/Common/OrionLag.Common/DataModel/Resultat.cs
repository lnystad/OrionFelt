namespace OrionLag.Common.DataModel
{
    using System;
    using System.Collections.Generic;

    class Resultat
    {
        public Guid SkytterId { get; set; }
        public Guid Id { get; set; }
        public Guid StevneId { get; set; }

        public List<Serie> Serier { get; set; }
    }
}
