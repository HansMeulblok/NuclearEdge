using UnityEngine;
using TMPro;

public class StartChunk : MonoBehaviour
{
  public GameObject startingLine;
  public TextMeshProUGUI text;

  private void Start() 
  {
    StartCoroutine(StartSequence());
  }

  IEnumerator StartSequence()
  {
    text.text = "";
        yield return null;
    
  }

}
