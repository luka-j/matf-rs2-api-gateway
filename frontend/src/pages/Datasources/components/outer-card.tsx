import React from "react";
import { PlusCircle } from "lucide-react";

import { Button } from "@/components/ui/button";
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from "@/components/ui/card";
import {
  Dialog,
  DialogContent,
  DialogDescription,
  DialogHeader,
  DialogTitle,
  DialogTrigger,
} from "@/components/ui/dialog";

import AddDatasourceForm from "./add-datasource-form";

interface IOuterCardProps {
  title: string;
  description: string;
  children: React.ReactNode;
}

const OuterCard = ({ title, description, children }: IOuterCardProps) => {
  return (
    <Card className="mx-4">
      <CardHeader>
        <CardTitle className="flex items-center gap-2">
          {title}
          <Dialog>
            <DialogTrigger asChild>
              <Button variant="ghost" size="icon" className="mt-1">
                <PlusCircle />
              </Button>
            </DialogTrigger>
            <DialogContent>
              <DialogHeader>
                <DialogTitle className="text-center text-xl">
                  Add a {title.slice(0, -1)}
                </DialogTitle>
                <DialogDescription>
                  Fill the form below to add a {title.slice(0, -1)}:
                </DialogDescription>
              </DialogHeader>
              <AddDatasourceForm type={title.slice(0, -1).toLowerCase()} />
            </DialogContent>
          </Dialog>
        </CardTitle>
        <CardDescription>{description}</CardDescription>
      </CardHeader>

      <CardContent className="flex flex-col items-center justify-between gap-8 md:flex-row">
        {children}
      </CardContent>
    </Card>
  );
};

export default OuterCard;
