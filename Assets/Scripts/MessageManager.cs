using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using TMPro;
using UniRx;
using UnityEngine;
using Zenject;

public class MessageManager : MonoBehaviour
{
    [Inject] private readonly TMP_Text messageText; 
    [SerializeField] private float messageDuration = 2f; 

    private CancellationTokenSource cts;

    private Dictionary<string, string> localizedMessages = new Dictionary<string, string>()
    {
        { "CubePlaced", "����� ����������" },
        { "CubeRemoved", "����� �����" },
        { "CubeFell", "����� ������" },
        { "HeightLimit", "���������� ����������� �� ������" }
    };

    private Subject<string> messageSubject = new Subject<string>(); 

    private void Start()
    {
        cts = new CancellationTokenSource();
        messageSubject
            .Subscribe(async messageKey => await DisplayMessage(messageKey))
            .AddTo(this);
    }

    public void ShowMessage(string messageKey)
    {
        if (!localizedMessages.ContainsKey(messageKey))
        {
            Debug.LogWarning($"��������� � ������ {messageKey} �� �������");
            return;
        }

        messageSubject.OnNext(messageKey);
    }

    private async Task DisplayMessage(string messageKey)
    {
        cts?.Cancel();
        cts = new CancellationTokenSource();
        var token = cts.Token;

        try
        {
            if (messageText == null) return;
            string message = localizedMessages[messageKey];

            messageText.text = message;
            messageText?.gameObject.SetActive(true);

            await Task.Delay(TimeSpan.FromSeconds(messageDuration), token);

            if (messageText != null)
            {
                messageText.gameObject.SetActive(false);
            }
        }
        catch (TaskCanceledException)
        {
            Debug.Log("����������� ��������� ��������");
        }
        finally
        {
            if (cts.Token == token)
            {
                cts.Dispose();
                cts = null;
            }
        }
    }
}
