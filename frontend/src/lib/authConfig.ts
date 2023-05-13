const authConfig = {
  authority: "https://rs2-re82fv.zitadel.cloud/",
  client_id: "213215184100065537@test",
  redirect_uri: "http://localhost:3000/callback",
  response_type: "code",
  scope: "openid profile email",
  post_logout_redirect_uri: "http://localhost:3000/",
  userinfo_endpoint: "https://rs2-re82fv.zitadel.cloud/oidc/v1/userinfo",
  response_mode: "query" as const,
  code_challenge_method: "S256",
};

export default authConfig;
