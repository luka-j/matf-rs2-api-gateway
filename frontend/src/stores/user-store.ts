import authConfig from "@/configs/auth-config";
import { User, UserManager, WebStorageStateStore } from "oidc-client-ts";
import { create } from "zustand";
import { devtools, persist } from "zustand/middleware";

const userManager = new UserManager({
  userStore: new WebStorageStateStore({ store: window.localStorage }),
  ...authConfig,
});

export interface IUserStore {
  currentUser: User | null;
  setCurrentUser: (user: User) => void;
  loginUser: () => void;
  logoutUser: () => void;
  loginAndSetUser: () => Promise<void>;
}

export const useUserStore = create<IUserStore>()(
  devtools(
    persist(
      (set, get) => ({
        currentUser: null,
        setCurrentUser: (user: User) => set({ currentUser: user }),
        loginUser: () => userManager.signinRedirect({ state: "a2123a67ff11413fa19217a9ea0fbad5" }),
        logoutUser: () => {
          userManager.signoutRedirect();
          set({ currentUser: null });
        },
        loginAndSetUser: async () => {
          return userManager.signinRedirectCallback().then((user) => {
            const access_token = user.access_token;
            if (user) get().setCurrentUser(user);
            localStorage.setItem("token", access_token);
          });
        },
      }),
      { name: "userStore", getStorage: () => sessionStorage },
    ),
  ),
);
