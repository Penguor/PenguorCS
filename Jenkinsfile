pipeline {
  agent {
    docker {
      image 'mcr.microsoft.com/dotnet/core/sdk'
    }

  }
  stages {
    stage('Prepare environment') {
      steps {
        sh 'dotnet --version'
      }
    }

    stage('Restore packages') {
      parallel {
        stage('Restore Penguor') {
          steps {
            sh 'dotnet restore ./Penguor'
          }
        }

        stage('Restore PDebug') {
          steps {
            sh 'dotnet restore ./PDebug'
          }
        }

      }
    }

    stage('Clean') {
      steps {
        sh '''dotnet clean ./Penguor
dotnet clean ./PDebug'''
      }
    }

    stage('Build') {
      steps {
        sh 'dotnet build ./Penguor'
      }
    }

    stage('Publish') {
      steps {
        sh 'dotnet publish ./Penguor -o /artifacts'
        archiveArtifacts(artifacts: '/artifacts', onlyIfSuccessful: true)
      }
    }

  }
  environment {
    DOTNET_CLI_HOME = '/tmp/DOTNET_CLI_HOME'
  }
}