pipeline {
    agent { 
        docker {
             image 'gableroux/unity3d:2019.2.8f1'
             args "-v ${env.WORKSPACE}:/root/project" 
        } 
    }
    options { 
        disableConcurrentBuilds()
        timeout(time: 1, unit: 'HOURS') 
    }
    environment {
        UNITY_LICENSE_CONTENT = """<?xml version="1.0" encoding="UTF-8"?><root>
    <License id="Terms">
        <MachineBindings>
            <Binding Key="1" Value="7d1999f293024b3999f74db825e2c3ba"/>
            <Binding Key="2" Value="7d1999f293024b3999f74db825e2c3ba"/>
        </MachineBindings>
        <MachineID Value="bQ2+0zd7Ajb5hdUREHNRRlbIH5c="/>
        <SerialHash Value="49ba21557d7ca962e9b4d41a335a4d71d35b23b5"/>
        <Features>
            <Feature Value="33"/>
            <Feature Value="1"/>
            <Feature Value="12"/>
            <Feature Value="2"/>
            <Feature Value="24"/>
            <Feature Value="3"/>
            <Feature Value="36"/>
            <Feature Value="17"/>
            <Feature Value="19"/>
            <Feature Value="62"/>
        </Features>
        <DeveloperData Value="AQAAAEY0LVVDVUMtVzI2Ui00Q1BCLTY0V0MtSkY1VQ=="/>
        <SerialMasked Value="F4-UCUC-W26R-4CPB-64WC-XXXX"/>
        <StartDate Value="2019-01-15T00:00:00"/>
        <UpdateDate Value="2019-12-20T10:43:25"/>
        <InitialActivationDate Value="2019-01-15T14:56:19"/>
        <LicenseVersion Value="6.x"/>
        <ClientProvidedVersion Value="2019.2.8f1"/>
        <AlwaysOnline Value="false"/>
        <Entitlements>
            <Entitlement Ns="unity_editor" Tag="UnityPersonal" Type="EDITOR" ValidTo="9999-12-31T00:00:00"/>
        </Entitlements>
    </License>
<Signature xmlns="http://www.w3.org/2000/09/xmldsig#"><SignedInfo><CanonicalizationMethod Algorithm="http://www.w3.org/TR/2001/REC-xml-c14n-20010315#WithComments"/><SignatureMethod Algorithm="http://www.w3.org/2000/09/xmldsig#rsa-sha1"/><Reference URI="#Terms"><Transforms><Transform Algorithm="http://www.w3.org/2000/09/xmldsig#enveloped-signature"/></Transforms><DigestMethod Algorithm="http://www.w3.org/2000/09/xmldsig#sha1"/><DigestValue>ouylUrk8Xc+WAd50LATn8WoBsT8=</DigestValue></Reference></SignedInfo><SignatureValue>Lnr9KjaA8F4DpvqnMc6bUm9V+lOTzDo1B8/Yp1F1G0wJft0J7Ym3qq4Y2lhpnVdG6l7Z/rGu8jFS
d4hTT57zzCoKslI4ju3v3OFWUXXTVC5cuLo1Z3sOIuAKEfvhCJI10EHDsdAOiqBvO37RSoKy25nk
HEswfsSa93WBqJaO//PMBs/aSou43l3Ev1h1IN1nTOHCkdT60HpzdiQTTBkERN07vNYvjkUJ8SdA
qxjZ/pmKRJ7nv8WB5spaexPeXRzNFiTypkhrOljo/J/S2kMj09RuHSpojAeAayzCc1zxK9gydnok
h5SaF1yG4MoMB1k1ck2D7SdKxnryaJWopnQylQ==</SignatureValue></Signature></root>"""
        UNITY_VERSION = 'UNITY_VERSION=2019.2.8f1'
        UNITY_USERNAME = credentials('Unity-username')
        UNITY_PASSWORD = credentials('Unity-password')
        SLACK_TOKEN = credentials('Slacktoken')
        TEST_PLATFORM = 'playmode'
        WORKDIR = '/root/project'
        TESTS_PASSED = 0
        TESTS_FAILED = 0
    }

    stages {
        stage('Unity-preparation') {
            steps {
                slackSend (channel: '#jenkins-notifications', color: '#4F8A10', message: "Started build: ${env.JOB_NAME} with number: ${env.BUILD_ID}")
                sh 'chmod +x ./preparation.sh && ./preparation.sh'
            }
        }
        stage('Test') {
            steps {
                sh 'chmod +x ./test.sh && ./test.sh'
            }
        }

        stage('BuildPlayers'){
            when{
                branch 'Develop'
            }
            steps{
                sh 'chmod +x ./build.sh && ./build.sh buildLinux64Player Linux '
                sh './build.sh buildOSX64Player MacOS /MacOSBuild.app'
                //sh './build.sh buildWindows64Player Windows /WindowsBuild.exe'
            }
        }
    }

    post {
        
        always {
            echo 'Done, moving to succes/failure stage'
            nunit testResultsPattern: "$TEST_PLATFORM-results.xml"
            slackUploadFile(filePath: "$TEST_PLATFORM-results.xml", channel: '#jenkins-notifications')
            archiveArtifacts artifacts: "gym/Linux/bin/**", fingerprint: true
        }
        success {
            echo 'succesfully' 
            slackSend (channel: '#jenkins-notifications', color: '#00FF00', message: "It ran succesfully! View results at: ${env.BUILD_URL} \n It passed a total of $TESTS_PASSED tests! \n See .xml file for the results: \n")
        }
        failure {
            echo 'wrongfully'
            slackSend (channel: '#jenkins-notifications', color: '#FF0000', message: "It failed... View errors at: ${env.BUILD_URL}\n View resultfile: \n")
            
        }
        
    }
}