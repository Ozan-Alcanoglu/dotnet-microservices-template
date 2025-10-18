pipeline {
    agent any
    
    environment {
        DOTNET_CLI_TELEMETRY_OPTOUT = '1'
    }
    
    stages {
        stage('Check .NET 9') {
            steps {
                sh 'dotnet --info'
            }
        }
        
        stage('Git Checkout') {
            steps {
                git branch: 'main',
                    url: 'https://github.com/Ozan-Alcanoglu/dotnet-microservices-template.git'
                sh 'ls -la'
            }
        }
        
        stage('Build') {
            steps {
                sh 'dotnet restore'
                sh 'dotnet build --configuration Release'
            }
        }
        
        stage('Test') {
            steps {
                sh 'dotnet test --verbosity normal'
            }
        }
    }
}
