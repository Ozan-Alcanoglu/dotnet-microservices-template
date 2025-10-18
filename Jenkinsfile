pipeline {
    agent {
        kubernetes {
            yaml '''
apiVersion: v1
kind: Pod
spec:
  containers:
  - name: dotnet
    image: mcr.microsoft.com/dotnet/sdk:9.0
    command: ['cat']
    tty: true
    resources:
      requests:
        memory: "1Gi"
        cpu: "500m"
'''
        }
    }
    
    stages {
        stage('Check .NET 9 in Kubernetes Pod') {
            steps {
                container('dotnet') {
                    sh 'dotnet --info'
                }
            }
        }
        
        stage('List Project Files') {
            steps {
                container('dotnet') {
                    sh 'ls -la'
                    sh 'find . -name "*.csproj" -o -name "*.sln" | head -10'
                }
            }
        }
        
        stage('Simple Build Test') {
            steps {
                container('dotnet') {
                    sh 'dotnet restore || echo "Restore failed"'
                    sh 'dotnet build || echo "Build failed"'
                }
            }
        }
    }
    
    post {
        always {
            echo '✅ Kubernetes Pod pipeline tamamlandı!'
        }
    }
}
