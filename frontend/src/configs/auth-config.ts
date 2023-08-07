import { OidcClientSettings } from "oidc-client-ts";

const FRONT_URL = import.meta.env.DEV ? "http://localhost:3000/" : "https://app-rs2.luka-j.rocks/";

const authConfig: OidcClientSettings = {
  authority: "https://rs2-re82fv.zitadel.cloud/",
  client_id: "213215184100065537@test",
  redirect_uri: `${FRONT_URL}callback`,
  response_type: "code",
  scope: "openid profile email",
  post_logout_redirect_uri: FRONT_URL,
  response_mode: "query" as const,
};

export default authConfig;
