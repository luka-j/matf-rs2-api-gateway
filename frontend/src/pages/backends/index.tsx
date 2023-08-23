import { PlusCircle, RefreshCcw, Trash } from "lucide-react";

import { backends } from "@/mock/backends";
import { Button } from "@/components/ui/button";
import { Card, CardContent, CardFooter, CardHeader, CardTitle } from "@/components/ui/card";
import { Typography } from "@/components/ui/typography";

const Backends = () => {
  return (
    <div className="container mt-8 flex flex-col gap-8">
      <div className="flex items-center justify-between">
        <Typography variant="h1">Backends</Typography>
        <div className="flex gap-8">
          <Button variant="ghost" size="icon">
            <RefreshCcw size={60} />
          </Button>
          <Button variant="ghost" size="icon">
            <PlusCircle size={60} />
          </Button>
        </div>
      </div>

      <Card>
        <CardContent className="flex flex-wrap justify-center gap-5 p-5 md:justify-between md:gap-10 md:p-10">
          {backends.map((backend) => (
            <Card key={backend.id} className="w-96">
              <CardHeader className="flex flex-row items-center justify-between">
                <CardTitle>{backend.name}</CardTitle>
                <Button variant="destructive" size="icon">
                  <Trash />
                </Button>
              </CardHeader>

              <CardContent className="flex flex-col justify-center gap-4">
                <Typography variant="large" className="text-center">
                  {backend.url}
                </Typography>
              </CardContent>

              <CardFooter className="flex flex-row items-center justify-center">
                <Button asChild variant="secondary">
                  <a href="/dasbhoard/spec">View spec</a>
                </Button>
              </CardFooter>
            </Card>
          ))}
        </CardContent>
      </Card>
    </div>
  );
};

export default Backends;
