using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class FormulaInfoSend : MonoBehaviour
{
    // Start is called before the first frame update

    private string player_id;
    private string chosenReseau = "Twitter";
    private string highscore;

    [SerializeField] private TMP_InputField playerIDInput;

    [SerializeField] private TextMeshProUGUI scoreText;
    public Animator formulaAnimator;
    public TextMeshProUGUI loadingText;

    private string BASE_URL = "https://docs.google.com/forms/u/5/d/e/1FAIpQLSce8xb_tjOwDYTPjJi0brAiGx1d08a_fJ2juMjPu2_oRC1USw/formResponse";
    void Start()
    {
        highscore = RandomTileManager.instance.GetScore().ToString();
        scoreText.text = highscore;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private IEnumerator Post(string name, string reseau, string score)
    {
        //formulaAnimator.SetTrigger("Loading");
        loadingText.text = "Envoi...";
        WWWForm form = new WWWForm();
        form.AddField("entry.687036189", name);
        form.AddField("entry.1938221334", reseau);
        form.AddField("entry.1800075712", score);
        byte[] rawData = form.data;
        WWW www = new WWW(BASE_URL, rawData);
        yield return new WaitForSeconds(3.5f);
        if(www.error != null)
        {
            formulaAnimator.SetTrigger("Fail");
            loadingText.text = "Erreur! Connexion introuvable";
            yield return null;
        }
        else
        {
            formulaAnimator.SetTrigger("Success");
            loadingText.text = "Reussi!";
            yield return www;
        }
        
    }

    public void SendInfo()
    {
        player_id = playerIDInput.text;
        if(player_id.Contains("=") || player_id.Length == 0)
        {
            //Do something else to show failed
            formulaAnimator.SetTrigger("Fail");
            loadingText.text = "Erreur! Identifiant invalide";
        }
        
        else StartCoroutine(Post(player_id, chosenReseau, highscore));
    }

    public void SetSocialMedia(string socialmedia)
    {
        chosenReseau = socialmedia;
    }
}
