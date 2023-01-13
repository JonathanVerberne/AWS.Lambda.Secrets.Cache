# AWS Lambda Secrets Cache Using Secrets Lambda Extension

This sample project consists of a test function that reads a secret key value pair from AWS Secrets Manager using the Secrets Lambda Extension.

## Install using AWS SAM CLI command line:

Once you have edited your template and code you can deploy your application using the [AWS SAM CLI Tool](https://docs.aws.amazon.com/serverless-application-model/latest/developerguide/serverless-sam-cli-command-reference.html) from the command line.

Install AWS SAM CLI if not already installed.
```
    https://docs.aws.amazon.com/serverless-application-model/latest/developerguide/install-sam-cli.html
```

If already installed check if new version is available.
```
    Follow steps to update cli version
    https://docs.aws.amazon.com/serverless-application-model/latest/developerguide/install-sam-cli.html

    Verify the installation.
    $ sam --version
```

Build & Deploy resources to AWS
```
    navigate to project folder in command prompt
    cd "AWS.Lambda.Secrets.Cache/src/AWS.Lambda.Secrets.Cache"

    Build package
    $ sam build

    Deploy package
    $ sam deploy -g
```

