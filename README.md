🚀 StaticBlaze Blog
A Blazor WebAssembly (WASM) blog platform that uses GitHub as a headless CMS.
✨ Deploy your blog for free on GitHub Pages! ✨

📝 Problem Statement
Managing blog content directly in a GitHub repository is manual and tedious.

Static site generators require local builds and lack real-time editing.

💡 Solution
Blazor WASM Frontend: Edit and preview posts in real-time.

GitHub as CMS: Push markdown and images to your repo via GitHub API.

Automated Deployment: GitHub Actions rebuilds and deploys on content changes.

🛠️ Tech Stack
Frontend:

🖥️ Blazor WASM

📝 Markdig (Markdown rendering)

✍️ Toast UI Editor (WYSIWYG Markdown editor)

🎨 Tailwind CSS (Styling)

Backend:

🌐 GitHub REST API

Deployment:

🚀 GitHub Pages

⚙️ GitHub Actions

🗺️ Roadmap
Paginated post listings with search

Tag-based categorization

Secure authentication via GitHub OAuth

Mermaid.js integration (for diagrams)

Post User comments (via GitHub Issues API)

Dark/Light theme toggle

SEO optimization

Theme and Appearance Customizability

🤝 Contributing
This project is at a very early planning stage, and your contributions are highly welcome! Here’s how you can help:

💬 Discuss: Share your thoughts on optimizations or ideas for doing things, potential security vulnerabilities, or better ways to implement things.

🐛 Report Issues: Found a bug? Let us know!

✨ Submit PRs: Contribute code or documentation.

🚀 Add Features: Help us expand the roadmap with new ideas.

🚀 Getting Started
Prerequisites:

🛠️ .NET 9.0

🔑 GitHub PAT with repo scope.

Setup:

bash
Copy
git clone https://github.com/SaintScraTchY/StaticBlaze.git
cd StaticBlaze
dotnet run
Configuration:

Update appsettings.json with your GitHub details.

Set up GitHub Secrets for your PAT.

📞 Contact
For questions, suggestions, or feedback, feel free to reach out:

Telegram: [Telegram](https://t.me/SaintScraTchY)

Email: Mehrshad2028@gmail.com

🌟 Why StaticBlaze?
Free Hosting: Deploy your blog on GitHub Pages at no cost.

Real-Time Editing: Write and preview posts in real-time.

GitHub-Powered: Use GitHub as your CMS for version control and collaboration.

Extensible: Add features like diagrams, comments, and themes.

📜 License
This project is licensed under the MIT License. See LICENSE for details.

🙏 Acknowledgments
Blazor Community for the amazing framework.

GitHub for providing free hosting and APIs.

You for checking out this project!

