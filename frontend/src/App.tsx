import { UserManager, WebStorageStateStore } from "oidc-client-ts";
import React, { useEffect, useMemo, useState } from "react";
import { BrowserRouter, Route, Routes } from "react-router-dom";

import authConfig from "./authConfig";
import Callback from "./components/Callback";
import Login from "./components/Login";

function App() {
  const userManager = useMemo(() => {
    return new UserManager({
      userStore: new WebStorageStateStore({ store: window.localStorage }),
      ...authConfig,
    });
  }, []);

  function authorize() {
    userManager.signinRedirect({ state: "a2123a67ff11413fa19217a9ea0fbad5" });
  }

  function clearAuth() {
    userManager.signoutRedirect();
  }

  const [authenticated, setAuthenticated] = useState<boolean | null>(null);
  const [userInfo, setUserInfo] = useState(null);

  useEffect(() => {
    userManager.getUser().then((user) => {
      if (user) {
        setAuthenticated(true);
      } else {
        setAuthenticated(false);
      }
    });
  }, [userManager]);

  return (
    <BrowserRouter>
      <Routes>
        <Route path="/" element={<Login auth={authenticated} handleLogin={authorize} />} />
        <Route
          path="/callback"
          element={
            <Callback
              auth={authenticated}
              setAuth={setAuthenticated}
              userInfo={userInfo}
              setUserInfo={setUserInfo}
              handleLogout={clearAuth}
              userManager={userManager}
            />
          }
        />
      </Routes>
    </BrowserRouter>
  );
}

export default App;
