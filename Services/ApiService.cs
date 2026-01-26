using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using TP203.Models;

namespace TP203.Services;

public class ApiService
{
    private static readonly HttpClient _httpClient;
    private const string BaseUrl = "https://192.168.137.189:5178/api";

    static ApiService()
    {
        // Bypass SSL certificate validation for local development
        var handler = new HttpClientHandler();
        handler.ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true;
        _httpClient = new HttpClient(handler);
    }

    // --- Authentication ---
    public static async Task<(User? user, string? error)> LoginAsync(string login, string password)
    {
        try
        {
            var loginData = new { login, password };
            var json = JsonConvert.SerializeObject(loginData);
            var body = new StringContent(json, Encoding.UTF8, "application/json");
            
            var response = await _httpClient.PostAsync($"{BaseUrl}/Utilisateur/login", body);
            
            var content = await response.Content.ReadAsStringAsync();
            
            if (response.IsSuccessStatusCode)
            {
                var user = JsonConvert.DeserializeObject<User>(content);
                return (user, null);
            }
            
            return (null, $"Serveur: {response.StatusCode} - {content}");
        }
        catch (Exception ex)
        {
            return (null, $"Erreur Reseau: {ex.Message}");
        }
    }

    // --- Séances ---
    public static async Task<List<Seance>> GetSeancesAsync()
    {
        try
        {
            var response = await _httpClient.GetAsync($"{BaseUrl}/séances");
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<List<Seance>>(content) ?? new List<Seance>();
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"GetSeances Error: {ex.Message}");
        }
        return new List<Seance>();
    }

    public static async Task<bool> CreateSeanceAsync(Seance seance)
    {
        try
        {
            // The API expects a specific format according to Swagger
            var dto = new
            {
                iD_Seance = 0, // Usually 0 for create
                date_Heure_Debut = seance.Date_Heure_Debut,
                date_Heure_Fin = seance.Date_Heure_Fin,
                ue = seance.UE_ID,
                enseignant = seance.Enseignant_ID,
                salle = seance.Salle_ID,
                niveau = seance.Niveau_ID,
                statut = seance.Statut ?? "PLANIFIEE"
            };

            var json = JsonConvert.SerializeObject(dto);
            var body = new StringContent(json, Encoding.UTF8, "application/json");

            // Warning: Accented URLs can be tricky. Using the exact string from swagger.
            var response = await _httpClient.PostAsync($"{BaseUrl}/séances", body);
            
            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                System.Diagnostics.Debug.WriteLine($"CreateSeance Failed: {response.StatusCode} - {error}");
            }

            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"CreateSeance Error: {ex.Message}");
            return false;
        }
    }

    public static async Task<bool> CreateSalleAsync(Salle salle)
    {
        try
        {
            var dto = new
            {
                nom = salle.Nom,
                type = salle.Type,
                capacite = salle.Capacite
            };
            var json = JsonConvert.SerializeObject(dto);
            var body = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync($"{BaseUrl}/Salle", body);
            return response.IsSuccessStatusCode;
        }
        catch { return false; }
    }

    public static async Task<bool> CreateUniteEnseignementAsync(UniteEnseignement ue)
    {
        try
        {
            var dto = new
            {
                code_UE = ue.Code_UE,
                titre = ue.Intitule,
                volume_Horaire_Total = ue.Volume_Horaire_Total,
                fK_Enseignant = ue.FK_Enseignant,
                fK_Classe = ue.FK_Classe
            };
            var json = JsonConvert.SerializeObject(dto);
            var body = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync($"{BaseUrl}/UniteEnseignement", body);
            return response.IsSuccessStatusCode;
        }
        catch { return false; }
    }

    public static async Task<bool> CreateNiveauAsync(Niveau niveau)
    {
        try
        {
            var dto = new
            {
                code = niveau.Code,
                annee_Academique = niveau.Annee_Academique,
                semestre = niveau.Semestre,
                effectif = niveau.Effectif,
                filiere = niveau.Filiere
            };
            var json = JsonConvert.SerializeObject(dto);
            var body = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync($"{BaseUrl}/Niveau", body);
            return response.IsSuccessStatusCode;
        }
        catch { return false; }
    }

    public static async Task<List<Filiere>> GetFilieresAsync()
    {
        try
        {
            var response = await _httpClient.GetAsync($"{BaseUrl}/Filiere");
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<List<Filiere>>(content) ?? new List<Filiere>();
            }
        }
        catch { }
        return new List<Filiere>();
    }

    // --- Reference Data ---
    public static async Task<List<Salle>> GetSallesAsync()
    {
        try
        {
            var response = await _httpClient.GetAsync($"{BaseUrl}/Salle");
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<List<Salle>>(content) ?? new List<Salle>();
            }
        }
        catch { }
        return new List<Salle>();
    }

    public static async Task<List<UniteEnseignement>> GetUEsAsync()
    {
        try
        {
            var response = await _httpClient.GetAsync($"{BaseUrl}/UniteEnseignement");
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<List<UniteEnseignement>>(content) ?? new List<UniteEnseignement>();
            }
        }
        catch { }
        return new List<UniteEnseignement>();
    }

    public static async Task<List<Niveau>> GetNiveauxAsync()
    {
        try
        {
            var response = await _httpClient.GetAsync($"{BaseUrl}/Niveau");
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<List<Niveau>>(content) ?? new List<Niveau>();
            }
        }
        catch { }
        return new List<Niveau>();
    }
}
