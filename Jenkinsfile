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
        sh 'dotnet publish ./Penguor'
      }
    }

    stage('Post-Clean') {
      steps {
        cleanWs(cleanWhenFailure: true, cleanWhenAborted: true, cleanWhenNotBuilt: true, cleanWhenSuccess: true, cleanWhenUnstable: true, cleanupMatrixParent: true, deleteDirs: true, disableDeferredWipeout: true)
      }
    }

  }
  environment {
    DOTNET_CLI_HOME = '/tmp/DOTNET_CLI_HOME'
  }
}