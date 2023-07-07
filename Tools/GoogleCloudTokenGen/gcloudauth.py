import json
import os
import jwt
import re

from typing import cast

from pathlib import Path
from google.oauth2 import service_account, credentials
from google.auth.transport.requests import Request
from google.auth.external_account_authorized_user import Credentials
from google_auth_oauthlib.flow import Flow, InstalledAppFlow

import base64
import hashlib
import requests

SERVICE_ACCOUNT_FILE_ENV_VAR = "CARLISTAPP_SERVICE_ACCOUNT_FILE"
SERVICE_ACCOUNT_DEFAULT_FILENAME = "service.json"
SCOPES = [
    'https://www.googleapis.com/auth/cloud-platform',
    'https://www.googleapis.com/auth/userinfo.email',
    'https://www.googleapis.com/auth/userinfo.profile',
    "openid"
]


TEST_SERVICE_CONFIG_FILENAME = "test_service.json"

def get_script_dir() -> Path :
    this_dir = Path(__file__).parent
    return this_dir

def generate_code_challenge(code_verifier : str) -> str :
    ascii_version = code_verifier.encode("utf-8")
    hashed_version = hashlib.sha256(ascii_version).digest()
    string_version = base64.urlsafe_b64encode(hashed_version).decode("utf-8").rstrip("=")
    return string_version

def generate_header(token : str) -> dict[str, str]:
    headers = {
        "Authorization" : "Bearer {}".format(token),
        "Accept" : "*/*"
    }
    return headers

def check_service_account_ok(target_audience) -> tuple[bool, str]:
    print("Authenticating using service account")
    this_dir = get_script_dir()
    service_account_file =  this_dir.joinpath("Config/" + SERVICE_ACCOUNT_DEFAULT_FILENAME)
    if SERVICE_ACCOUNT_FILE_ENV_VAR in os.environ :
        service_account_file = Path(os.environ[SERVICE_ACCOUNT_FILE_ENV_VAR])

    if not service_account_file.exists() :
        print("Service account file {} does not exist.".format(service_account_file))
        return (False, "")

    credentials = service_account.IDTokenCredentials.from_service_account_file(service_account_file, target_audience=target_audience)
    request = Request()
    credentials.refresh(request)
    token = credentials.token

    headers = generate_header(token)
    response = requests.get(target_audience, headers=headers)
    if response.status_code != 200 :
        print("Failed to authenticate service account")
        return (False, "")

    decoded_token = jwt.decode(token,  options={"verify_signature": False})
    print("Successfully retrieved token for service account {}\n\n".format(decoded_token["email"]))
    return (True, token)

def check_client_can_request_token(target_audience : str) -> tuple[bool, str] :
    print("Authenticating using InstalledAppFlow auto OAuth handling")
    this_dir = get_script_dir()
    client_secrets_file = this_dir.joinpath("Config/client_secrets.json")

    if not client_secrets_file.exists() :
        print("Client secrets file does not exist")
        return (False, "")

    redirect_uris : list[str] = []
    with open(client_secrets_file, 'r') as file :
        content = json.load(file)
        for uri in content["web"]["redirect_uris"] :
            redirect_uris.append(uri)
    
    flow = InstalledAppFlow.from_client_secrets_file(client_secrets_file,
                                                     scopes=SCOPES)
    
    https_candidates = [x for x in redirect_uris if "https" in x]
    pattern = re.compile('https?:\/\/(\w*):([0-9]*)')
    
    candidate_uri = https_candidates[0] if len(https_candidates) != 0 else [x for x in redirect_uris if "http" in x][0]
    matches = re.findall(pattern, candidate_uri)
    (hostname, port_number) = matches[0]
    creds = flow.run_local_server(host="localhost", port=int(port_number)) # type: ignore

    #print("Proprietary google OAUTH2 token : {}".format(flow.credentials.token))
    creds = cast(Credentials, flow.credentials)
    creds.audience = target_audience #type:ignore
    

    #headers = generate_header(creds.id_token) #type:ignore
    #response = requests.get(target_audience, headers=headers)
    #if response.status_code != 200 :
    #    print("Failed to authenticate service account")
    #    return (False, "")

    decoded_token = jwt.decode(creds.id_token, options={"verify_signature": False}) #type:ignore
    print("Successfully retrieved token for user {}\n\n".format(decoded_token["name"]))
    return (True, creds.id_token) #type:ignore


def main():
    this_dir = Path(__file__).parent

    # Will be replaced by actual Cloud Run service endpoint from config file
    target_audience = "default"

    test_service_filepath = this_dir.joinpath("Config/" + TEST_SERVICE_CONFIG_FILENAME)
    with open(test_service_filepath, "r") as file :
        content = json.load(file)
        target_audience = content["name"]

    (success, token) = check_client_can_request_token(target_audience)
    if success :
        print(token)

if __name__ == "__main__" :
    exit(main())