import { PlusCircle, RefreshCcw, Trash } from "lucide-react";

import { frontends } from "@/mock/frontends";
import { Button } from "@/components/ui/button";
import { Card, CardContent, CardFooter, CardHeader, CardTitle } from "@/components/ui/card";
import { Typography } from "@/components/ui/typography";

const Frontends = () => {
  return (
    <div className="container mt-8 flex flex-col gap-8">
      <div className="flex items-center justify-between">
        <Typography variant="h1">Frontends</Typography>
        <Button variant="ghost" size="icon">
          <PlusCircle size={60} />
        </Button>
      </div>

      <Card>
        <CardContent className="flex flex-wrap justify-center gap-5 p-5 md:justify-between md:gap-10 md:p-10">
          {frontends.map((frontend) => (
            <Card key={frontend.id} className="w-96">
              <CardHeader className="flex flex-row items-center justify-between">
                <Button variant="ghost" size="icon">
                  <RefreshCcw />
                </Button>
                <CardTitle>{frontend.name}</CardTitle>
                <Button variant="destructive" size="icon">
                  <Trash />
                </Button>
              </CardHeader>

              <CardContent className="flex flex-col justify-center gap-4">
                <Typography variant="large" className="text-center">
                  {frontend.url}
                </Typography>
                <Typography variant="small" className="text-center">
                  {frontend.endpoints} endpoints
                </Typography>
              </CardContent>

              <CardFooter className="flex flex-row items-center justify-between">
                <Button asChild variant="secondary">
                  <a href="/">View spec</a>
                </Button>
                <Button asChild variant="secondary">
                  <a href="/">View middleware</a>
                </Button>
              </CardFooter>
            </Card>
          ))}
        </CardContent>
      </Card>
    </div>
  );
};

export default Frontends;
