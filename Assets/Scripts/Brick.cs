using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Brick : MonoBehaviour
{
    public UnityEvent<int> onDestroyed;
    public GameObject BrickExplosionPrefab;
    public AudioSource audioSource;
    public AudioClip audioClipBoom;

    public int PointValue;
   
    void Start()
    {   

    var renderer = GetComponentInChildren<Renderer>();

        MaterialPropertyBlock block = new MaterialPropertyBlock();
        switch (PointValue)
        {
            case 1 :
                block.SetColor("_BaseColor", Color.green);
                break;
            case 2:
                block.SetColor("_BaseColor", Color.yellow);
                break;
            case 5:
                block.SetColor("_BaseColor", Color.blue);
                break;
            default:
                block.SetColor("_BaseColor", Color.red);
                break;
        }
        renderer.SetPropertyBlock(block);
    }

    private void OnCollisionEnter(Collision other)
    {        
        onDestroyed.Invoke(PointValue);
        Boom();
        //slight delay to be sure the ball have time to bounce
        Destroy(gameObject, 0.2f);
    }

    void Boom()
    {
        // Instantiate the brick explosion prefab
        GameObject explosion = Instantiate(BrickExplosionPrefab, transform.position, Quaternion.identity);

        FindAudioSource();

        audioSource.PlayOneShot(audioClipBoom);

        // If the explosion has a particle system
        var particleSystem = explosion.GetComponent<ParticleSystem>();
        if (particleSystem != null)
        {
            // Access the main module to set the start color
            var mainModule = particleSystem.main;
            switch (PointValue)
            {
                case 1:
                    mainModule.startColor = Color.green;
                    LightenBrickColor(Color.green);
                    break;
                case 2:
                    mainModule.startColor = Color.yellow;
                    LightenBrickColor(Color.yellow);
                    break;
                case 5:
                    mainModule.startColor = Color.blue;
                    LightenBrickColor(Color.blue);
                    break;
                default:
                    mainModule.startColor = Color.red;
                    LightenBrickColor(Color.red);
                    break;
            }
        }
    }

    void FindAudioSource()
    {
        audioSource = null; // Clear the variable to ensure it's properly assigned
        foreach (AudioSource source in FindObjectsOfType<AudioSource>())
        {
            if (source.name == "sfx")
            {
                audioSource = source;
                break;
            }
        }

        if (audioSource == null)
        {
            Debug.LogError("AudioSource with the specified name not found.");
        }
    }

    void LightenBrickColor(Color originalColor)
    {
        // Get the renderer component
        var renderer = GetComponentInChildren<Renderer>();

        // Calculate a lighter version of the original color
        Color lightenedColor = originalColor * 1.5f; // You can adjust the multiplier as needed

        // Set the new color to the brick
        MaterialPropertyBlock block = new MaterialPropertyBlock();
        block.SetColor("_BaseColor", lightenedColor);
        renderer.SetPropertyBlock(block);
    }

}
