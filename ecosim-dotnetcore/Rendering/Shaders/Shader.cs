using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using static ecosim_dotnetcore.OpenGL.GL;

namespace ecosim_dotnetcore.Rendering.Shaders
{
    class Shader
    {
        string vertexCode;
        string fragmentCode;

        public uint programId { get; set; }

        public Shader(string vertexCode, string fragmentCode)
        {
            this.vertexCode = vertexCode;
            this.fragmentCode = fragmentCode;
        }

        public void Load()
        {
            uint vs, fs;

            vs = glCreateShader(GL_VERTEX_SHADER);
            glShaderSource(vs, vertexCode);
            glCompileShader(vs);

            int[] status = glGetShaderiv(vs, GL_COMPILE_STATUS, 1);

            if(status[0] == 0)
            {
                string error = glGetShaderInfoLog(vs);
                Debug.WriteLine("ERROR COMPILING VERTEX SHADER: " + error);
            }

            fs = glCreateShader(GL_FRAGMENT_SHADER);
            glShaderSource(fs, fragmentCode);
            glCompileShader(fs);

            status = glGetShaderiv(fs, GL_COMPILE_STATUS, 1);

            if (status[0] == 0)
            {
                string error = glGetShaderInfoLog(fs);
                Debug.WriteLine("ERROR COMPILING FRAGMENT SHADER: " + error);
            }

            programId = glCreateProgram();
            glAttachShader(programId, vs);
            glAttachShader(programId, fs);

            glLinkProgram(programId);

            glDetachShader(programId, vs);
            glDetachShader(programId, vs);

            glDeleteShader(vs);
            glDeleteShader(fs);
        }
        public void Use()
        {
            glUseProgram(programId);
        }
    }
}
