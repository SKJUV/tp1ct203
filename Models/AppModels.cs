using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.ObjectModel;
using Newtonsoft.Json;

namespace TP203.Models;

public enum UserType
{
    Enseignant = 1,
    Etudiant = 2,
    Delegue = 3,
    Admin = 4 // Chef de département
}

public enum SeanceType
{
    CM, 
    TP,
    CC,
    TD,
    EXAM
}

// ======================= BACKEND MODELS =======================

public class Departement
{
    [Key, JsonProperty("iD_Departement")]
    public int ID_Departement { get; set; }
    
    [JsonProperty("nom")]
    public string Nom { get; set; }
    
    [JsonProperty("code")]
    public string Code { get; set; }
}

public class Filiere
{
    [Key, JsonProperty("iD_Filiere")]
    public int ID_Filiere { get; set; }
    
    [JsonProperty("code")]
    public string Code { get; set; }
    
    [JsonProperty("intitule")]
    public string Intitule { get; set; }
    
    [JsonProperty("departement")]
    public int Departement { get; set; }
}

public class Niveau
{
    [Key, JsonProperty("id_Niveau")]
    public int Id_Niveau { get; set; }
    
    [JsonProperty("code")]
    public string Code { get; set; } // Ex: ICT-L2
    
    [JsonProperty("annee_Academique")]
    public string Annee_Academique { get; set; }
    
    [JsonProperty("semestre")]
    public int Semestre { get; set; }
    
    [JsonProperty("effectif")]
    public int Effectif { get; set; }
    
    [JsonProperty("filiere")]
    public int Filiere { get; set; }
    
    // Frontend Alias
    [NotMapped, JsonIgnore] public int Id => Id_Niveau;
}

// Alias pour le code existant qui utilise "Classe"
public class Classe : Niveau 
{
    // Wrapper for compatibility
}

public class Utilisateur
{
    [Key, JsonProperty("iD_Utilisateur")]
    public int ID_Utilisateur { get; set; }
    
    [JsonProperty("login")]
    public string Login { get; set; }
    
    [JsonProperty("type")]
    public int Type { get; set; } // ADMIN, ENSEIGNANT, ETUDIANT
    
    [JsonProperty("actif")]
    public bool Actif { get; set; }

    // Frontend Compatibility Properties
    [NotMapped, JsonIgnore] public int Id => ID_Utilisateur;
    [NotMapped, JsonIgnore] public string Name { get; set; } = string.Empty; 
    [NotMapped, JsonIgnore] public string Email { get; set; } = string.Empty;
    [NotMapped, JsonIgnore] 
    public UserType Role 
    { 
        get => (UserType)Type;
        set => Type = (int)value; 
    }
}

// Alias pour le code existant qui utilise "User"
public class User : Utilisateur { }


public class Etudiant
{
    [Key, JsonProperty("iD_Etudiant")]
    public int ID_Etudiant { get; set; }
    
    [JsonProperty("matricule")]
    public string Matricule { get; set; }
    
    [JsonProperty("nom")]
    public string Nom { get; set; }
    
    [JsonProperty("prenom")]
    public string Prenom { get; set; }
    
    [JsonProperty("niveau")]
    public int Niveau { get; set; }
    
    [JsonProperty("fK_Utilisateur")]
    public int FK_Utilisateur { get; set; }
}

public class Salle
{
    [Key, JsonProperty("iD_Salle")]
    public int ID_Salle { get; set; }
    
    [JsonProperty("nom")]
    public string Nom { get; set; } = string.Empty;
    
    [JsonProperty("type")]
    public string Type { get; set; } = "Amphi"; // Amphi, Salle, Labo
    
    [JsonProperty("capacite")]
    public int Capacite { get; set; }

    // Frontend Alias
    [NotMapped, JsonIgnore] public int Id => ID_Salle;
}

public class UniteEnseignement
{
    [Key, JsonProperty("iD_UE")]
    public int ID_UE { get; set; }
    
    [JsonProperty("code_UE")]
    public string Code_UE { get; set; } = string.Empty;
    
    [JsonProperty("titre")]
    public string Intitule { get; set; } = string.Empty;
    
    [JsonProperty("volume_Horaire_Total")]
    public int Volume_Horaire_Total { get; set; }
    
    [JsonProperty("fK_Enseignant")]
    public int FK_Enseignant { get; set; }
    
    [JsonProperty("fK_Classe")]
    public int FK_Classe { get; set; }

    // Frontend Compatibility
    [NotMapped, JsonIgnore] public int Id => ID_UE;
    [NotMapped, JsonIgnore] public string Code { get { return Code_UE; } set { Code_UE = value; } }
    [NotMapped, JsonIgnore] public string Nom { get { return Intitule; } set { Intitule = value; } }
    
    [NotMapped, JsonIgnore] 
    public double VolumeHoraireTotal 
    { 
        get => Volume_Horaire_Total; 
        set => Volume_Horaire_Total = (int)value; 
    }

    [NotMapped, JsonIgnore] public double VolumeHoraireEffectue { get; set; }
    
    [NotMapped, JsonIgnore] public double Progression => VolumeHoraireTotal > 0 ? (VolumeHoraireEffectue / VolumeHoraireTotal) * 100 : 0;
}


public class SeanceCours
{
    [Key, JsonProperty("iD_Seance")]
    public int ID_Seance { get; set; }
    
    [JsonProperty("date_Heure_Debut")]
    public DateTime Date_Heure_Debut { get; set; }
    
    [JsonProperty("date_Heure_Fin")]
    public DateTime Date_Heure_Fin { get; set; }
    
    [JsonProperty("ue")]
    public int UE_ID { get; set; }
    
    [JsonProperty("enseignant")]
    public int Enseignant_ID { get; set; }
    
    [JsonProperty("salle")]
    public int Salle_ID { get; set; }
    
    [JsonProperty("niveau")]
    public int Niveau_ID { get; set; }
    
    [JsonProperty("statut")]
    public string Statut { get; set; } = "PLANIFIEE"; // PLANIFIEE, ANNULEE, VALIDEE

    // ================== FRONTEND NAVIGATION & ALIASES ==================
    // Ces propriétés servent à l'affichage XAML et simulent les jointures
    
    [NotMapped] public int Id => ID_Seance;

    // Helpers de temps dérivés de Date_Heure_Debut/Fin
    [NotMapped] 
    public DateTime Date 
    { 
        get => Date_Heure_Debut.Date; 
        set 
        { 
            // Update Date part only
            Date_Heure_Debut = value.Date + Date_Heure_Debut.TimeOfDay;
            Date_Heure_Fin = value.Date + Date_Heure_Fin.TimeOfDay;
        }
    }

    [NotMapped]
    public TimeSpan HeureDebut
    {
        get => Date_Heure_Debut.TimeOfDay;
        set => Date_Heure_Debut = Date.Date + value;
    }

    [NotMapped]
    public TimeSpan HeureFin
    {
        get => Date_Heure_Fin.TimeOfDay;
        set => Date_Heure_Fin = Date.Date + value;
    }

    [NotMapped] public SeanceType Type { get; set; } = SeanceType.CM; // Pas dans la table backend? Ajout temporaire

    // Navigation Objects (Full Objects for Binding)
    [NotMapped] public UniteEnseignement UE { get; set; } = new();
    [NotMapped] public User Enseignant { get; set; } = new();
    [NotMapped] public Salle Salle { get; set; } = new();
    [NotMapped] public Classe Classe { get; set; } = new(); // Mapped to Niveau

    // Display Helpers
    [NotMapped] public string PlageHoraire => $"{HeureDebut:hh\\:mm} - {HeureFin:hh\\:mm}";
    [NotMapped] public string DisplayDate => Date.ToString("ddd. dd MMM");
    [NotMapped] public bool IsValidatedByProf { get; set; }
    [NotMapped] public bool IsValidatedByDelegue { get; set; }
}

// Alias pour le code existant qui utilise "Seance"
public class Seance : SeanceCours { }
