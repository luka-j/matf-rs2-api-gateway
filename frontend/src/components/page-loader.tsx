import { Loader2 } from "lucide-react";

const PageLoader = () => {
  return (
    <div className="flex h-screen w-screen items-center justify-center overflow-hidden">
      <Loader2 size={100} className="animate-spin stroke-foreground" />
      <span className="sr-only">Loading...</span>
    </div>
  );
};

export default PageLoader;
