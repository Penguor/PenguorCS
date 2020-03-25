pipeline {
  agent {
    docker {
      image 'mcr.microsoft.com/dotnet/core/sdk'
    }

  }
  stages {
    stage('Restore') {
      steps {
        sh 'dotnet --version'
      }
    }

    stage('error') {
      steps {
        sh 'dotnet restore'
      }
    }

  }
}