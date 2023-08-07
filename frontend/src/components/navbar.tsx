import { useUserStore } from "@/stores/user-store";

import ModeToggle from "./mode-toggle";
import { Button } from "./ui/button";
import { Typography } from "./ui/typography";

const Navbar = () => {
  const { logoutUser, currentUser } = useUserStore((state) => state);

  return (
    <header className="supports-backdrop-blur:bg-background/60 sticky top-0 z-40 w-full border-b bg-background/95 backdrop-blur">
      <div className="container flex h-14 items-center">
        <a href="/">
          <Typography variant="large">API Gateway</Typography>
        </a>
        <div className="flex flex-1 items-center justify-end space-x-2">
          <nav className="flex items-center gap-4">
            <ModeToggle />
            {currentUser && (
              <Button variant="secondary" onClick={logoutUser}>
                Logout
              </Button>
            )}
          </nav>
        </div>
      </div>
    </header>
  );
};

export default Navbar;
