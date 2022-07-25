using ecosim_dotnetcore.Rendering.Display;
using System;
using System.Collections.Generic;
using GLFW;
using static ecosim_dotnetcore.OpenGL.GL;
using ecosim_dotnetcore.Rendering.Shaders;

namespace ecosim_dotnetcore
{
    class Simulator
    {
        SimulatorTime timer;
        List<Ecosystem> ecosystems;
        float[] vertices = { 
                -0.001f, -0.001f, 50f, 10f, 1f, 0f, 0f, 0.5f,
                -0.001f, 0.001f, 50f, 10f, 0f, 1f, 0f, 0.5f,
                0.001f, -0.001f, 50f, 10f, 0f, 0f, 1f, 0.5f,
                0.001f, 0.001f, 50f, 10f, 1f, 1f, 1f, 0.5f,
            };
        float[] backGroundVertex = {
            -1.0f, -1.0f, 0.0f,
            1.0f, -1.0f, 0.0f,
            1.0f, 1.0f, 0.0f,
            -1.0f, 1.0f, 0.0f,
            0.0f, 1.0f, 0.0f
        };

        bool gameRun = true;

        uint vao;
        uint vbo;
        Shader frameBufferShader;
        Shader backgroundShader;
        Shader agentShader;
        Shader agentVisionShader;

        public void RunSimulation(bool runWithGrafics, int ecosystems, double timeLimit, Dictionary<string, dynamic> paramsToChange)
        {
                Initialize(ecosystems, paramsToChange);
                DisplayManager.CreatWindow(Cache.GetConfiguration("ENGINE_WINDOW_X"), Cache.GetConfiguration("ENGINE_WINDOW_Y"), "ecosim");
                if (runWithGrafics)
                    LoadContent();

                while ((!Glfw.WindowShouldClose(DisplayManager.window)) && (timeLimit == 0 || timer.totalElapsedSeconds < timeLimit ))
                {
                    timer.totalElapsedSeconds = Glfw.Time;
                    if (gameRun && timer.totalElapsedSeconds >= timer.lastUpdateTime + (1.0 / Cache.GetConfiguration("ENGINE_FPS")))
                    {
                        timer.lastUpdateTime = timer.totalElapsedSeconds;
                        for (int i = 0; i < ecosystems; i++)
                        {
                            Update(i);
                        }
                    }
                    if (runWithGrafics)
                    {
                        Glfw.PollEvents();
                        Render();
                    }
                        
                }
                DisplayManager.CloseWindow();
        }
        protected void Initialize(int ecossystems, Dictionary<string, dynamic> paramsToChange)
        {
            int ecossystemId;
            ecosystems = new List<Ecosystem>();
            Cache.ecossytemData = new Dictionary<int, EcossystemData>();
            for (int i = 0; i < ecossystems; i++)
            {
                ecossystemId = i;
                Cache.ecossytemData.Add(ecossystemId, new EcossystemData());
                ecosystems.Add(new Ecosystem(ecossystemId, paramsToChange));
                timer = new SimulatorTime();
            }
        }
        protected void LoadContent()
        {
            LoadFrameBufferShader();
            LoadBackGroundShader();
            LoadAgentShader();
            LoadAgentVisionShader();
        }
        protected unsafe void LoadFrameBufferShader()
        {
            string vertexShader = @"#version 130
                                    in vec4 position;
                                    in vec4 color_in;
                                    out vec4 color_out;
                                    out vec4 pos_out;
                                    void main()
                                    {
                                        pos_out = position;
                                        color_out = color_in;
                                        gl_Position = gl_ModelViewProjectionMatrix * position;
                                    }";

            string fragmentShader = @"#version 130
                                    in vec4 color_out;
                                    in vec4 pos_out;
                                    uniform sampler2D fbo_texture;
                                    uniform vec2 pos_offset;
                                    uniform float zoom;

                                    vec2 offset_tex(vec2 t_pos, vec2 wobble, float off_x, float off_y)
                                    {
                                        vec2 offset = vec2(0.0, 0.0);
                                        offset += t_pos;
                                        offset += wobble;
                                        offset += vec2(off_x, off_y);
                                        return offset;
                                    }

                                    vec2 calc_wobble(vec2 cam_offset, vec2 pos, float zoom)
                                    {
                                        vec2 wobble = vec2(0.0, 0.0);
                                        wobble.x = max(0, sin((cam_offset.y - pos.y / zoom)*16)) * 0.008;
                                        wobble.y = max(0, sin((cam_offset.x - pos.x / zoom)*24)) * 0.008;
                                        wobble *= zoom;
                                        return wobble;
                                    }

                                    void main()
                                    {
                                        vec4 t_sum = vec4(0.0);
                                        vec2 wobble;
                                        vec2 offset = vec2(0.0035, 0.0035);
                                        float blur_amt = 0.5 / 8;
                                        vec2 t_pos = vec2(0.5, 0.5);
                                        t_pos *= pos_out.xy;
                                        t_pos += vec2(0.5, 0.5);

                                        /* Create wobble for warping agents */
                                        wobble = calc_wobble(pos_offset.xy, pos_out.xy, zoom);

                                        /* Add offset textures, clearing wobble and blur effect */
                                        t_sum += texture2D(fbo_texture,
                                        offset_tex(t_pos, wobble, offset.x, offset.y)) * blur_amt;
                                        t_sum += texture2D(fbo_texture,
                                        offset_tex(t_pos, wobble, -offset.x, offset.y)) * blur_amt;
                                        t_sum += texture2D(fbo_texture,
                                        offset_tex(t_pos, wobble, -offset.x, -offset.y)) * blur_amt;
                                        t_sum += texture2D(fbo_texture,
                                        offset_tex(t_pos, wobble, offset.x, -offset.y)) * blur_amt;

                                        offset *= 2.0;
                                        blur_amt *= 0.75;

                                        t_sum += texture2D(fbo_texture,
                                        offset_tex(t_pos, wobble, offset.x, offset.y)) * blur_amt;
                                        t_sum += texture2D(fbo_texture,
                                        offset_tex(t_pos, wobble, -offset.x, offset.y)) * blur_amt;
                                        t_sum += texture2D(fbo_texture,
                                        offset_tex(t_pos, wobble, -offset.x, -offset.y)) * blur_amt;
                                        t_sum += texture2D(fbo_texture,
                                        offset_tex(t_pos, wobble, offset.x, -offset.y)) * blur_amt;

                                        t_sum += texture2D(fbo_texture,
                                        offset_tex(t_pos, wobble, 0.0, 0.0)) * 1.0;
                                        gl_FragColor = t_sum;
                                    }";


            frameBufferShader = new Shader(vertexShader, fragmentShader);
            frameBufferShader.Load();
        }
        protected unsafe void LoadBackGroundShader()
        {
            string vertexShader = @"#version 130
                                    in vec4 position;
                                    in vec4 color_in;
                                    uniform float time;
                                    out vec4 color_out;
                                    out vec4 pos_out;

                                    void main()
                                    {
                                        pos_out = position;
                                        color_out = color_in;
                                        gl_Position = gl_ModelViewProjectionMatrix * position;
                                    }";

            string fragmentShader = @"#version 130
                                    in vec4 color_out;
                                    in vec4 pos_out;
                                    uniform float time;

                                    float rand(float n)
                                    {
                                        return fract(sin(n + fract(time)) * 43758.5453123);
                                    }

                                    void main()
                                    {
                                        vec2 pos = pos_out.xy;
                                        vec4 new_col = vec4(1.0, 1.0, 1.0, 1.0);
                                        float noise = rand(pos.x * pos.y) * 0.02;
                                        new_col.a = noise;
                                        gl_FragColor = new_col;
                                    }";

            backgroundShader = new Shader(vertexShader, fragmentShader);
            backgroundShader.Load();
        }
        protected unsafe void LoadAgentShader() 
        {
            string vertexShader = @"#version 130
                                    attribute vec4 position;
                                    attribute vec4 color;
                                    uniform vec2 window; 
                                    uniform float zoom;
                                    out vec4 color_out;

                                    void main()
                                    {
                                        color_out = color;
                                        gl_Position = gl_ModelViewProjectionMatrix * vec4(position.x, position.y, 0.0, 1.0);
                                        gl_PointSize = position.z * window.x * zoom * 1.0;
                                    }";

            string fragmentShader = @"#version 130
                                    out vec4 frag_colour;
                                    in vec4 color_out;

                                    float rand(float n)
                                    {
                                        return fract(sin(n) * 43758.5453123);
                                    }

                                    void main()
                                    {
                                        float agent_state = color_out.w;
                                        if(agent_state == -1)
                                        {
                                            gl_FragColor = vec4(0.0);
                                            return;
                                        }
                                        vec4 color = color_out;
                                        color.w = 0.9;
                                        vec2 pos = gl_PointCoord - vec2(0.5);
                                        /* Gen pattern */
                                        float pat_r = length(pos)*1.0;
                                        float pat_a = atan(pos.y,pos.x);
                                        float pat_f = cos(pat_a * 80);
                                        float alpha =  smoothstep(pat_f,pat_f+0.02,pat_r) * 1.0;
                                        float cut_r = 1.0;

                                        /* Gen circle */
                                        float cutoff = 1.0 - smoothstep(cut_r - (cut_r * 0.02), cut_r + (cut_r * 0.02), dot(pos, pos) * 4.0);
                                        gl_FragColor =  alpha * cutoff * color * ((1.0 - (length(pos) )) * 1.5) ;
                                    }";

            agentShader = new Shader(vertexShader, fragmentShader);
            agentShader.Load();
        }
        protected unsafe void LoadAgentVisionShader()
        {
            string vertexShader = @"#version 130
                                    attribute vec4 position;
                                    attribute vec4 color;
                                    uniform vec2 window;
                                    out vec4 color_out;
                                    uniform float zoom;

                                    void main()
                                    {
                                        color_out = color;
                                        gl_Position = gl_ModelViewProjectionMatrix * vec4(position.x, position.y, 0.0, 1.0);
                                        gl_PointSize = position.w * window.x * zoom;
                                    }";

            string fragmentShader = @"#version 130
                                    out vec4 frag_colour;
                                    in vec4 color_out;
                                    uniform float zoom;

                                    float rand(float n){return fract(sin(n) * 43758.5453123);}
                                    float mini_rand(float n){return fract(sin(n) * 150);}

                                    void main()
                                    {
                                        float agent_state = color_out.w;
                                        if(agent_state == -1)
                                        {
                                            gl_FragColor = vec4(0.0);
                                            return;
                                        }
                                        vec4 color = color_out;
                                        color.w = 0.2;
                                        vec2 pos = gl_PointCoord - vec2(0.5);
                                        float noise = rand(pos.x * pos.y) * 0.07;
                                        color.w = color.w + noise;

                                        float radius = 1.0; //0.25;;
                                        float cutoff = 1.0 - smoothstep(radius - (radius * 0.01),
                                        radius + (radius * 0.01),
                                        dot(pos, pos) * 4.0);

                                        float pat_r = length(pos) * 1.0; 
                                        float pat_a = atan(pos.y,pos.x);
                                        float pat_f = cos(pat_a * 80);
                                        float alpha =  smoothstep(pat_f,pat_f+0.02,pat_r) * 0.1;

                                        float noise_pat = mini_rand((pos.x + 0.5) * (pos.y + 0.5));
                                        float ripple = fract((length(pos) - 0.4) * 4.0);
                                        color += vec4(noise_pat * 0.15);

                                        gl_FragColor =  alpha * cutoff * color * ((1.0 - (length(pos) )) * 1.5);
                                    }";

            agentVisionShader = new Shader(vertexShader, fragmentShader);
            agentVisionShader.Load();
        }
        protected void Update(int ecosytemId)
        {
            ecosystems[ecosytemId].RunInteration(timer.totalElapsedSeconds);
            if (ecosytemId == 0)
                vertices = Cache.ecossytemData[ecosytemId].metrics.GetVertex();
        }
        protected unsafe void Render()
        {
            RenderAgentVision();

            RenderFrameBuffer();
        }
        protected unsafe void RenderFrameBuffer()
        {
   
        }
        protected unsafe void RenderBackGround()
        {

            vao = glGenVertexArray();
            vbo = glGenBuffer();

            glBindVertexArray(vao);

            glBindBuffer(GL_ARRAY_BUFFER, vbo);

            fixed (float* v = &backGroundVertex[0])
            {
                glBufferData(GL_ARRAY_BUFFER, sizeof(float) * backGroundVertex.Length, v, GL_STATIC_DRAW);
            }
            glVertexAttribPointer(0, 3, GL_FLOAT, false, 3 * sizeof(float), (void*)0);
            glEnableVertexAttribArray(0);

            glVertexAttribPointer(1, 3, GL_FLOAT, false, 3 * sizeof(float), (void*)(9 * sizeof(float)));
            glEnableVertexAttribArray(1);

            glBindBuffer(GL_ARRAY_BUFFER, 0);
            glBindVertexArray(0);

            glClearColor(0, 0, 0, 0);
            glClear(GL_COLOR_BUFFER_BIT);

            backgroundShader.Use();

            int loc = glGetUniformLocation(backgroundShader.programId, "time");
            glUniform1f(loc, (float)timer.totalElapsedSeconds);

            glBindVertexArray(vao);
            glBindVertexArray(0);
        }
        protected unsafe void RenderAgent()
        {
            vao = glGenVertexArray();
            vbo = glGenBuffer();

            glBindVertexArray(vao);

            glBindBuffer(GL_ARRAY_BUFFER, vbo);

            fixed (float* v = &vertices[0])
            {
                glBufferData(GL_ARRAY_BUFFER, sizeof(float) * vertices.Length, v, GL_STATIC_DRAW);
            }
            glVertexAttribPointer(0, 4, GL_FLOAT, false, 8 * sizeof(float), (void*)0);
            glEnableVertexAttribArray(0);

            glVertexAttribPointer(1, 4, GL_FLOAT, false, 8 * sizeof(float), (void*)(4 * sizeof(float)));
            glEnableVertexAttribArray(1);

            glBindBuffer(GL_ARRAY_BUFFER, 0);
            glBindVertexArray(0);

            glClearColor(0, 0, 0, 0);
            glClear(GL_COLOR_BUFFER_BIT);

            agentShader.Use();

            int loc = glGetUniformLocation(agentShader.programId, "window");
            glUniform2f(loc, 1.0f, 1.0f);

            int loc2 = glGetUniformLocation(agentShader.programId, "zoom");
            glUniform1f(loc2, 1.0f);

            glBindVertexArray(vao);
            glPointSize(7f);
            glDrawArrays(GL_POINTS, 0, vertices.Length / 8);


            glBindVertexArray(0);

            Glfw.SwapBuffers(DisplayManager.window);
        }
        protected unsafe void RenderAgentVision()
        {
            vao = glGenVertexArray();
            vbo = glGenBuffer();

            glBindVertexArray(vao);

            glBindBuffer(GL_ARRAY_BUFFER, vbo);

            fixed (float* v = &vertices[0])
            {
                glBufferData(GL_ARRAY_BUFFER, sizeof(float) * vertices.Length, v, GL_STATIC_DRAW);
            }
            glVertexAttribPointer(0, 4, GL_FLOAT, false, 8 * sizeof(float), (void*)0);
            glEnableVertexAttribArray(0);

            glVertexAttribPointer(1, 4, GL_FLOAT, false, 8 * sizeof(float), (void*)(4 * sizeof(float)));
            glEnableVertexAttribArray(1);

            glBindBuffer(GL_ARRAY_BUFFER, 0);
            glBindVertexArray(0);

            glClearColor(0, 0, 0, 0);
            glClear(GL_COLOR_BUFFER_BIT);

            glBindVertexArray(vao);


            int loc = glGetUniformLocation(agentVisionShader.programId, "window");
            glUniform2f(loc, 1.0f, 1.0f);

            int loc2 = glGetUniformLocation(agentVisionShader.programId, "zoom");
            glUniform1f(loc2, 1.0f);

            agentVisionShader.Use();
            glPointSize(35f);
            glDrawArrays(GL_POINTS, 0, vertices.Length / 8);

            int loc3 = glGetUniformLocation(agentShader.programId, "window");
            glUniform2f(loc, 1.0f, 1.0f);

            int loc4 = glGetUniformLocation(agentShader.programId, "zoom");
            glUniform1f(loc2, 1.0f);

            agentShader.Use();
            glPointSize(15f);
            glDrawArrays(GL_POINTS, 0, vertices.Length / 8);

            glBindVertexArray(0);

            Glfw.SwapBuffers(DisplayManager.window);
        }
    }
}
