pipeline {
  agent {
    docker {
      image 'mcr.microsoft.com/dotnet/core/sdk'
    }

  }
  stages {
    stage('Check versions') {
      steps {
        sh 'dotnet --version'
      }
    }

    stage('Build') {
      steps {
        dir(path: './Penguor/') {
          sh 'dotnet restore'
        }

      }
    }

  }
  environment {
    DOTNET_CLI_HOME = '/tmp/DOTNET_CLI_HOME'
  }
}