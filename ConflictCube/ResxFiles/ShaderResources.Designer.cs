﻿//------------------------------------------------------------------------------
// <auto-generated>
//     Dieser Code wurde von einem Tool generiert.
//     Laufzeitversion:4.0.30319.42000
//
//     Änderungen an dieser Datei können falsches Verhalten verursachen und gehen verloren, wenn
//     der Code erneut generiert wird.
// </auto-generated>
//------------------------------------------------------------------------------

namespace ConflictCube.ResxFiles {
    using System;
    
    
    /// <summary>
    ///   Eine stark typisierte Ressourcenklasse zum Suchen von lokalisierten Zeichenfolgen usw.
    /// </summary>
    // Diese Klasse wurde von der StronglyTypedResourceBuilder automatisch generiert
    // -Klasse über ein Tool wie ResGen oder Visual Studio automatisch generiert.
    // Um einen Member hinzuzufügen oder zu entfernen, bearbeiten Sie die .ResX-Datei und führen dann ResGen
    // mit der /str-Option erneut aus, oder Sie erstellen Ihr VS-Projekt neu.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "15.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class ShaderResources {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal ShaderResources() {
        }
        
        /// <summary>
        ///   Gibt die zwischengespeicherte ResourceManager-Instanz zurück, die von dieser Klasse verwendet wird.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("ConflictCube.ResxFiles.ShaderResources", typeof(ShaderResources).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   Überschreibt die CurrentUICulture-Eigenschaft des aktuellen Threads für alle
        ///   Ressourcenzuordnungen, die diese stark typisierte Ressourcenklasse verwenden.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Sucht eine lokalisierte Zeichenfolge, die #version 330
        ///
        ///uniform vec2 iResolution; //[xResolution, yResolution] of display
        ///uniform float iGlobalTime; //global time in seconds as float
        ///uniform float startTime; //Time the shader was first activated on this object
        ///uniform float direction; //Positive =&gt; up/right; Negative =&gt; down/left
        ///uniform float lifetime; //How long the effect will be visible
        ///uniform vec3 desiredColor;
        ///	
        ///in vec2 uv;
        ///
        ///void main()
        ///{
        ///    //create uv to be in the range [0..1]x[0..1]
        ///	//vec2 uv = gl_FragCoord.xy / iResolutio [Rest der Zeichenfolge wurde abgeschnitten]&quot;; ähnelt.
        /// </summary>
        internal static string Afterglow {
            get {
                return ResourceManager.GetString("Afterglow", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Sucht eine lokalisierte Zeichenfolge, die #version 330
        ///
        ///uniform vec2 iResolution; //[xResolution, yResolution] of display
        ///uniform float iGlobalTime; //global time in seconds as float
        ///uniform float startTime; //Time the shader was first activated on this object
        ///uniform float direction; //Positive =&gt; up/right; Negative =&gt; down/left
        ///uniform float lifetime; //How long the effect will be visible
        ///uniform vec3 desiredColor;
        ///	
        ///in vec2 uv;
        ///
        ///void main()
        ///{
        ///    gl_FragColor = vec4(vec3(min(distance(uv, vec2(1)), distance(uv, vec2(0)))), 1);
        ///} ähnelt.
        /// </summary>
        internal static string FragmentTest {
            get {
                return ResourceManager.GetString("FragmentTest", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Sucht eine lokalisierte Zeichenfolge, die #version 330
        ///
        ///uniform vec2 iResolution; //[xResolution, yResolution] of display
        ///uniform float iGlobalTime; //global time in seconds as float
        ///uniform sampler2D tex;
        ///uniform float minXuv;
        ///uniform float maxXuv;
        ///uniform float minYuv;
        ///uniform float maxYuv;
        ///
        ///
        ///in vec2 pos;
        ///in vec2 uv;
        ///
        ///float rand(float seed)
        ///{
        ///	return fract(sin(seed) * 1231534.9);
        ///}
        ///
        ///vec2 scaleBack(in vec2 toScale) 
        ///{
        ///    vec2 scaledVec = vec2(0);
        ///    scaledVec.x = toScale.x * (maxXuv - minXuv) + minXuv;
        ///    scaledVec.y = to [Rest der Zeichenfolge wurde abgeschnitten]&quot;; ähnelt.
        /// </summary>
        internal static string Liquid {
            get {
                return ResourceManager.GetString("Liquid", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Sucht eine lokalisierte Zeichenfolge, die #version 330
        ///
        ///uniform mat4 camera;
        ///
        ///in vec2 uvPos;
        ///in vec2 position;
        ///
        ///out vec2 uv;
        ///out vec2 pos;
        ///	
        ///void main()
        ///{
        ///    uv = uvPos;
        ///    pos = position;
        ///    gl_Position = camera * vec4(position, 0, 1);
        ///    
        ///    //gl_Position = vec4(-position, 0, 1);
        ///} ähnelt.
        /// </summary>
        internal static string StandardVertexShader {
            get {
                return ResourceManager.GetString("StandardVertexShader", resourceCulture);
            }
        }
    }
}
