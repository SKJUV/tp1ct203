using System;
using System.Collections.Generic;
using TP203.Models;

namespace TP203.Services;

public static class DataProvider
{
    public static List<Seance> AllSeances { get; private set; } = new();

    // Données de référence pour les formulaires
    public static List<string> UEs => new() { "ICT203", "ICT204", "MAT201", "ENG101", "CMP302", "LAW101", "ORG202" };
    public static List<string> Enseignants => new() { "Dr. Kemgou", "Pr. Fofana", "Mme. Smith", "Dr. Z", "Dr. Y", "M. Tchuente" };
    public static List<string> Salles => new() { "Amphi 100", "Amphi 200", "Amphi 500", "Salle A", "Salle B", "Labo Cisco", "Labo Info 1" };
    public static List<string> Filieres => new() { "ICT-L1", "ICT-L2", "ICT-L3", "Master 1" };
    public static List<string> Types => new() { "CM", "TP", "TD", "CC", "Exam" };

    static DataProvider()
    {
        InitializeMockData();
    }

    public static void AddSeance(Seance seance)
    {
        AllSeances.Add(seance);
    }

    private static void InitializeMockData()
    {
        DateTime today = DateTime.Today;
        DateTime startOfWeek = today.AddDays(-(int)today.DayOfWeek + 1); // Lundi

        // --- ICT-L2 ---
        AllSeances.Add(Create("ICT203", "Génie Logiciel", "Dr. Kemgou", "Amphi 100", startOfWeek.AddDays(0), 8, 10, SeanceType.CM, "ICT-L2")); // Lundi 8h
        AllSeances.Add(Create("ENG201", "Anglais Tech", "Mme. Smith", "Salle B", startOfWeek.AddDays(0), 10, 12, SeanceType.TP, "ICT-L2"));   // Lundi 10h
        AllSeances.Add(Create("MAT201", "Proba Stat", "Pr. Fofana", "Amphi 200", startOfWeek.AddDays(1), 14, 16, SeanceType.CM, "ICT-L2"));  // Mardi 14h

        // --- ICT-L3 ---
        AllSeances.Add(Create("ICT305", "Réseaux Avancés", "Dr. Z", "Labo Cisco", startOfWeek.AddDays(0), 14, 17, SeanceType.TP, "ICT-L3")); // Lundi 14h
        AllSeances.Add(Create("MGT301", "Gestion Projet", "Dr. Y", "Salle A", startOfWeek.AddDays(2), 8, 11, SeanceType.CM, "ICT-L3"));      // Mercredi 8h

        // --- ICT-L1 ---
        AllSeances.Add(Create("ALG101", "Algo 1", "Pr. Fofana", "Amphi 500", startOfWeek.AddDays(2), 8, 10, SeanceType.CM, "ICT-L1"));       // Mercredi 8h (Fofana enseigne aussi ici !)
    }

    private static Seance Create(string code, string nomUE, string prof, string salle, DateTime date, int hDeb, int hFin, SeanceType type, string classe)
    {
        return new Seance
        {
            UE = new UniteEnseignement { Code = code, Nom = nomUE },
            Enseignant = new User { Name = prof },
            Salle = new Salle { Nom = salle },
            Date = date,
            HeureDebut = new TimeSpan(hDeb, 0, 0),
            HeureFin = new TimeSpan(hFin, 0, 0),
            Type = type,
            Classe = new Classe { Code = classe }
        };
    }
}
