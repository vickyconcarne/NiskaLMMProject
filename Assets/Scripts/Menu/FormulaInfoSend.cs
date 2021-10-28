using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Networking;

public class FormulaInfoSend : MonoBehaviour
{
    // Start is called before the first frame update

    private string player_id;
    private string chosenReseau = "Twitter";
    private string highscore;

    [SerializeField] private TMP_InputField playerIDInput;

    [SerializeField] private TextMeshProUGUI scoreText;

    public Image twitterButton;
    public Sprite selectedTwitter;
    public Sprite unselectedTwitter;
    public Image instagramButton;
    public Sprite selectedInsta;
    public Sprite unselectedInsta;
    public Image snapchatButton;
    public Sprite selectedSnap;
    public Sprite unselectedSnap;
    public Animator formulaAnimator;
    public TextMeshProUGUI loadingText;

    //private string BASE_URL = "https://docs.google.com/forms/u/5/d/e/1FAIpQLSce8xb_tjOwDYTPjJi0brAiGx1d08a_fJ2juMjPu2_oRC1USw/formResponse";
    private string BASE_URL = "https://docs.google.com/forms/d/e/1FAIpQLSe3DQetXV9wG5DC6Fd8YggtaibHiGu__MLzPME39SoTbrmx5Q/formResponse";
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
        form.AddField("entry.454267212", name);
        form.AddField("entry.163988641", reseau); //1938221334
        form.AddField("entry.1255323953", score); //1800075712
        byte[] rawData = form.data;
        WWW www = new WWW(BASE_URL, rawData);
        //UnityWebRequest uwr = UnityWebRequest.Post(BASE_URL, form);
        yield return new WaitForSeconds(3.5f);
        if(www.error != null /*uwr.result == UnityWebRequest.Result.ConnectionError || uwr.result == UnityWebRequest.Result.ProtocolError*/)
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
            //yield return uwr;
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

    public void ReselectSocialMedia()
    {
        
        twitterButton.sprite = unselectedTwitter;
        instagramButton.sprite = unselectedInsta;
        snapchatButton.sprite = unselectedSnap;
        switch (chosenReseau)
        {
            case "Twitter":
                twitterButton.sprite = selectedTwitter;
                break;
            case "Instagram":
                instagramButton.sprite = selectedInsta;
                break;
            case "Snapchat":
                snapchatButton.sprite = selectedSnap;
                break;
        }
    }
}
